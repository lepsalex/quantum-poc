using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Photon.Hive.Plugin;
using Quantum.model;
using WebSocket4Net;

namespace Quantum
{
  class QuantumCustomPluginFactory : IPluginFactory2
  {
    private IPluginFiber _globalFiber;
    private BunchWebsocket _websocket;
    private Dictionary<string, CustomQuantumPlugin> _plugins = new Dictionary<string, CustomQuantumPlugin>();

    public IGamePlugin Create(IPluginHost gameHost, String pluginName, Dictionary<String, String> config, out String errorMsg)
    {
      var server = new CustomQuantumServer(config);
      var plugin = new CustomQuantumPlugin(server, _globalFiber, OnGameClose);

      // You can inject fiber instance into server and plugin here

      InitLog(plugin);
      if (plugin.SetupInstance(gameHost, config, out errorMsg))
      {
        OnGameOpen(plugin);
        return plugin;
      }

      return null;
    }

    public void SetFactoryHost(IFactoryHost factoryHost, FactoryParams factoryParams)
    {
      // Called once when server is initialized
      _globalFiber = factoryHost.CreateFiber();
      _websocket = new BunchWebsocket();

      // _globalFiber.Enqueue(() =>
      // {
      //
      // });
      // _globalFiber.CreateTimer(() => { /* DO SOMETHING */ }, 5000, 5000);
    }
    

    private void OnGameOpen(CustomQuantumPlugin plugin)
    {
      _plugins.Add(plugin.PluginHost.GameId, plugin);
      _websocket.SendRoomOpenMessage(plugin.PluginHost.GameId);
    }

    private void OnGameClose(String gameId)
    {
      _plugins.Remove(gameId);
      _websocket.SendRoomClosedMessage(gameId);
    }
    
    private void InitLog(DeterministicPlugin plugin)
    {
      Log.Init(
        info => { plugin.LogInfo(info); },
        warn => { plugin.LogWarning(warn); },
        error => { plugin.LogError(error); },
        exn => { plugin.LogFatal(exn.ToString()); }
      );

      DeterministicLog.Init(
        info => { plugin.LogInfo(info); },
        warn => { plugin.LogWarning(warn); },
        error => { plugin.LogError(error); },
        exn => { plugin.LogFatal(exn.ToString()); }
      );
    }
  }
}
