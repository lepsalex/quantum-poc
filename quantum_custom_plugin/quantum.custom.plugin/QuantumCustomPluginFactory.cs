using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Photon.Hive.Plugin;
using Quantum.Backend;
using Quantum.Model;

namespace Quantum
{
  class QuantumCustomPluginFactory : IPluginFactory2
  {
    private IPluginFiber _globalFiber;
    private WebsocketConnection _websocketConnection;
    private Dictionary<string, CustomQuantumPlugin> _pluginRegistry = new Dictionary<string, CustomQuantumPlugin>();

    public IGamePlugin Create(IPluginHost gameHost, String pluginName, Dictionary<String, String> config,
      out String errorMsg)
    {
      var server = new CustomQuantumServer(config);
      var plugin = new CustomQuantumPlugin(server, _globalFiber, OnCloseGameInstance);

      // You can inject fiber instance into server and plugin here
      InitLog(plugin);
      if (plugin.SetupInstance(gameHost, config, out errorMsg))
      {
        OnCreateGameInstance(plugin);
        return plugin;
      }

      return null;
    }

    public void SetFactoryHost(IFactoryHost factoryHost, FactoryParams factoryParams)
    {
      // Called once when server is initialized
      _globalFiber = factoryHost.CreateFiber();
      _websocketConnection = new WebsocketConnection(OnCommandMessage);
    }


    /**
     * Called from QuantumCustomPluginFactory::Create method
     * on successful creation of a plugin instance
     */
    private void OnCreateGameInstance(CustomQuantumPlugin plugin)
    {
      // register plugin into registry
      _pluginRegistry.Add(plugin.PluginHost.GameId, plugin);
      // notify backend via websocket connection of game create
      _websocketConnection.SendRoomOpenMessage(plugin.PluginHost.GameId);
    }

    /**
     * Passed to the plugin instance to be enqueued into the global
     * IPluginFiber when the room is closing
     */
    private void OnCloseGameInstance(String gameId)
    {
      // remove plugin from registry
      _pluginRegistry.Remove(gameId);
      // notify plugin via websocket of game close
      _websocketConnection.SendRoomClosedMessage(gameId);
    }

    /**
     * Handler for messages received via the websocket connection
     * of the type RoomCommandMessage
     */
    private void OnCommandMessage(RoomCommandMessage msg)
    {
      // Set targetPluginInstance if exists else return early
      if (!_pluginRegistry.TryGetValue(msg.RoomId, out var targetPluginInstance)) return;
      
      // convert the RoomCommandMessage to a DeterministicCommand
      // if not possible return early
      if(!msg.TryGetDeterministicCommand(out var command)) return;

      // If a command is present, enqueue the send
      // to the targetPluginInstance room fiber
      targetPluginInstance.PluginHost.GetRoomFiber().Enqueue(() =>
      {
        targetPluginInstance.SendDeterministicCommand(command);
      });
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