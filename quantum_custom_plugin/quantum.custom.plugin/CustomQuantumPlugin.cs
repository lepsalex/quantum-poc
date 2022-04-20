using System.Collections.Generic;
using Photon.Deterministic;
using Photon.Deterministic.Server.Interface;
using Photon.Hive.Plugin;
using Quantum.CustomState;

namespace Quantum {
  public class CustomQuantumPlugin : DeterministicPlugin {
    protected CustomQuantumServer _server;

    public CustomQuantumPlugin(IServer server) : base(server) {
      Assert.Check(server is CustomQuantumServer);
      _server = (CustomQuantumServer)server;
    }
    
    public override void OnCloseGame(ICloseGameCallInfo info) {
      _server.Dispose();
      var lastFrame = (Frame) _server.GetVerifiedFrame();
      var players = new List<PlayerState>();
      
      foreach (var (entity, playerLink) in lastFrame.GetComponentIterator<PlayerLink>())
      {
        var playerTransform = lastFrame.Get<Transform3D>(entity);
        players.Add(new PlayerState(playerLink, playerTransform));
      }
      
      // Serialize and send players data to backend
      
      base.OnCloseGame(info);
    }
  }
}