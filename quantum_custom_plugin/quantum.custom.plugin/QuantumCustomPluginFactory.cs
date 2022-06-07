using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Photon.Hive.Plugin;
using Quantum.Backend;
using Quantum.CustomState.Commands;
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

    private void OnCommandMessage(RoomCommandMessage msg)
    {
      // get target room from message
      var targetRoom = msg.RoomId;

      // exit if room does not exist in registry
      if (!_pluginRegistry.ContainsKey(targetRoom))
      {
        return;
      }

      // make command based on message type
      DeterministicCommand command = null;

      switch (msg.Type)
      {
        case "CommandChangePlayerColor":
          command = new CommandChangePlayerColor()
          {
            Player = Convert.ToInt32(msg.Data["Player"]),
            NewColorIndex = Convert.ToInt32(msg.Data["NewColorIndex"])
          };
          break;
        case "CommandRemovePlayer":
          command = new CommandRemovePlayer()
          {
            Player = Convert.ToInt32(msg.Data["Player"])
          };
          break;
      }

      if (command != null)
      {
        _pluginRegistry[targetRoom].PluginHost.GetRoomFiber().Enqueue(() =>
        {
          _pluginRegistry[targetRoom].SendDeterministicCommand(command);
        });
      }
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