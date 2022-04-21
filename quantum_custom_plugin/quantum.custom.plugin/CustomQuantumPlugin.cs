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

    public override void OnCreateGame(ICreateGameCallInfo info)
    {
      // Get any existing state for the game (if it exists) and restore
      var exampleStates =
        "[{\"PlayerRef\":{\"_index\":1,\"IsValid\":true},\"PlayerX\":{\"RawValue\":-171180,\"AsLong\":-3,\"AsInt\":-3,\"AsShort\":-3,\"AsFloat\":-2.61199951,\"AsDouble\":-2.61199951171875},\"PlayerY\":{\"RawValue\":65485,\"AsLong\":0,\"AsInt\":0,\"AsShort\":0,\"AsFloat\":0.9992218,\"AsDouble\":0.9992218017578125},\"PlayerZ\":{\"RawValue\":125740,\"AsLong\":1,\"AsInt\":1,\"AsShort\":1,\"AsFloat\":1.91864014,\"AsDouble\":1.91864013671875}}]";
      var playerStates = JsonConvert.DeserializeObject<List<PlayerState>>(exampleStates);

      var restorePlayersCommand = new CommandRestorePlayerStates()
      {
        PlayerStates = playerStates
      };

      _server.SendDeterministicCommand(restorePlayersCommand);

      base.OnCreateGame(info);
    }

    public override void OnCloseGame(ICloseGameCallInfo info)
    {;
      var lastFrame = _server.GetVerifiedFrame();
      var playerStates = new List<PlayerState>();

      foreach (var (entity, playerLink) in lastFrame.GetComponentIterator<PlayerLink>())
      {
        var playerTransform = lastFrame.Get<Transform3D>(entity);

        playerStates.Add(new PlayerState()
        {
          PlayerRef = playerLink.PlayerRef,
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