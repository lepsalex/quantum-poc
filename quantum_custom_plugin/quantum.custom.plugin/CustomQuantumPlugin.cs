﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Photon.Deterministic;
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
      // in the real appm we wll use custom auth that will give us access to
      // info.authCookie which we can pass to the backend again to auth room join
      // ref: https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/authentication/custom-authentication
      var userAuthToken = info.Nickname;

      // should use PluginHost.GameId instead of BackendServer.DemoRoomName but need to work on unity side for that
      var roomId = BackendServer.DemoRoomName;

      // make the http request with the provided callback that defers continuing until after the data is loaded
      var roomRestoreRequest = _backendServer.RoomRestoreRequest(userAuthToken, roomId, OnCreateGameCallback);
      PluginHost.HttpRequest(roomRestoreRequest, info);
    }

    private void OnCreateGameCallback(IHttpResponse response, object state)
    {
      switch (response.HttpCode)
      {
        case 200:
          Log.Debug("Room restore request successful!");
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

          break;
        case 401:
          throw new UnauthorizedAccessException("Room restore request unauthorized!");
        case 403:
          throw new UnauthorizedAccessException("Room restore request forbidden!");
      }

      base.OnCreateGame((ICreateGameCallInfo) response.CallInfo);
    }

    public override void BeforeJoin(IBeforeJoinGameCallInfo info)
    {
      // in the real appm we wll use custom auth that will give us access to
      // info.authCookie which we can pass to the backend again to auth room join
      // ref: https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/authentication/custom-authentication
      var userAuthToken = info.Nickname;

      // should use PluginHost.GameId instead of BackendServer.DemoRoomName but need to work on unity side for that
      var roomId = BackendServer.DemoRoomName;

      var roomAuthRequest = _backendServer.RoomAuthRequest(userAuthToken, roomId, BeforeJoinCallback);
      PluginHost.HttpRequest(roomAuthRequest, info);
    }

    private void BeforeJoinCallback(IHttpResponse response, object state)
    {
      // copying from base method we are no longer calling
      Debug.Assert(!this.fireAssert || this.PluginHost != null);

      switch (response.HttpCode)
      {
        case 200:
          response.CallInfo.Continue();
          break;
        case 401:
          response.CallInfo.Fail();
          break;
        case 403:
          response.CallInfo.Fail();
          break;
      }
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
      var roomSaveRequest = _backendServer.RoomSaveRequest(roomState, OnCloseGameCallback);
      PluginHost.HttpRequest(roomSaveRequest, info);
    }

    private void OnCloseGameCallback(IHttpResponse response, object state)
    {
      // TODO: Handle request issues/retries here
      switch (response.HttpCode)
      {
        case 201:
          Log.Debug("Room save request successful!");
          break;
        case 401:
          Log.Debug("Room save request unauthorized!");
          break;
        case 403:
          Log.Debug("Room save request forbidden!");
          break;
      }

      // Dispose server and call base class
      _server.Dispose();
      base.OnCloseGame((ICloseGameCallInfo) response.CallInfo);
    }
  }
}