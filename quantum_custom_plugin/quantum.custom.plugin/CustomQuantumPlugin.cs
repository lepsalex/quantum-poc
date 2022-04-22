using System.Collections.Generic;
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

    public CustomQuantumPlugin(IServer server) : base(server)
    {
      Assert.Check(server is CustomQuantumServer);
      _server = (CustomQuantumServer) server;
    }

    public override void OnCloseGame(ICloseGameCallInfo info)
    {;
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

      // Serialize and send players data to backend
      var roomStateJson = JsonConvert.SerializeObject(playerStates);
      
      // Dispose server and call base class
      _server.Dispose();
      base.OnCloseGame(info);
    }
  }
}