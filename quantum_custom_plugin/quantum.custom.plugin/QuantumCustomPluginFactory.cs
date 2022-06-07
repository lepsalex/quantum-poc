using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Deterministic;
using Photon.Hive.Plugin;
using Quantum.CustomState.Commands;
using Quantum.model;
using WebSocket4Net;

namespace Quantum
{
  class QuantumCustomPluginFactory : IPluginFactory2
  {
    private IPluginFiber _globalFiber;
    private BunchWebsocket _websocket;
    private Dictionary<string, CustomQuantumPlugin> _plugins = new Dictionary<string, CustomQuantumPlugin>();

    public IGamePlugin Create(IPluginHost gameHost, String pluginName, Dictionary<String, String> config,
      out String errorMsg)
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
      _websocket = new BunchWebsocket(OnCommandMessage);
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

    private void OnCommandMessage(RoomCommandMessage msg)
    {
      // get target room from message
      var targetRoom = msg.RoomId;

      // exit if room does not exist in registry
      if (!_plugins.ContainsKey(targetRoom))
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
        _plugins[targetRoom].PluginHost.GetRoomFiber().Enqueue(() =>
        {
          _plugins[targetRoom].SendDeterministicCommand(command);
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