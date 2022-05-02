using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.Deterministic;
using Photon.Deterministic.Protocol;
using Photon.Deterministic.Server.Interface;
using Photon.Hive.Plugin;
using Quantum.CustomState;
using Quantum.CustomState.Commands;

namespace Quantum
{
  public class CustomQuantumPlugin : DeterministicPlugin
  {
    protected CustomQuantumServer _server;

    private BackendServer _backendServer;

    public CustomQuantumPlugin(IServer server) : base(server)
    {
      Assert.Check(server is CustomQuantumServer);
      _server = (CustomQuantumServer) server;
      _backendServer = new BackendServer();
    }

    public override void OnCreateGame(ICreateGameCallInfo info)
    {
      // should use pluginHost.PluginHost.GameId instead of BackendServer.DemoRoomName but need to work on unity side for that
      var roomId = BackendServer.DemoRoomName;
      
      // make the http request with the provided callback that defers continuing until after the data is loaded
      var roomRestoreRequest = _backendServer.roomRestoreRequest(roomId, OnCreateGameCallback);
      PluginHost.HttpRequest(roomRestoreRequest, info);
    }

    private void OnCreateGameCallback(IHttpResponse response, object state)
    {
      if (response.Status.Equals(HttpRequestQueueResult.Success))
      {
        // Get PlayerState list from JSON Response
        var roomState = JsonConvert.DeserializeObject<RoomState>(response.ResponseText);
        var playerStates = JsonConvert.DeserializeObject<List<PlayerState>>(roomState.playerStates);

        // Map PlayerState to CommandRestorePlayerState
        var playerRestoreCommands = playerStates.Select(playerState => new CommandRestorePlayerState()
        {
          PlayerRef = playerState.PlayerRef,
          PlayerPrototype = playerState.PlayerPrototype,
          PlayerX = playerState.PlayerX,
          PlayerY = playerState.PlayerY,
          PlayerZ = playerState.PlayerZ
        });

        // Send commands in sequence
        foreach (var command in playerRestoreCommands)
        {
          _server.StartupCommands.Add(command);
        }
      }
      
      base.OnCreateGame((ICreateGameCallInfo) response.CallInfo);
    }


    public override void OnCloseGame(ICloseGameCallInfo info)
    {
      var lastFrame = _server.GetVerifiedFrame();
      var playerStates = new List<PlayerState>();

      foreach (var (entity, playerLink) in lastFrame.GetComponentIterator<PlayerLink>())
      {
        var runtimePlayer = lastFrame.GetPlayerData(playerLink.PlayerRef);
        var playerPrototype = lastFrame.FindAsset<EntityPrototype>(runtimePlayer.CharacterPrototype.Id);
        var playerTransform = lastFrame.Get<Transform3D>(entity);

        playerStates.Add(new PlayerState()
        {
          PlayerRef = playerLink.PlayerRef,
          PlayerPrototype = playerPrototype,
          PlayerX = playerTransform.Position.X,
          PlayerY = playerTransform.Position.Y,
          PlayerZ = playerTransform.Position.Z
        });
      }

      var roomState = new RoomState()
      {
        roomId = BackendServer.DemoRoomName, // should use PluginHost.GameId but need to work on unity side for that
        playerStates = JsonConvert.SerializeObject(playerStates)
      };

      // Serialize and send players data to backend
      var blockingRoomSaveCall = _backendServer.roomSaveRequest(roomState);
      PluginHost.HttpRequest(blockingRoomSaveCall, info);

      // Dispose server and call base class
      _server.Dispose();
      base.OnCloseGame(info);
    }
  }
}