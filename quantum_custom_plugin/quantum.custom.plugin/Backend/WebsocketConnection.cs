using System;
using System.Timers;
using Newtonsoft.Json;
using Quantum.Model;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Quantum.Backend
{
  public class WebsocketConnection
  {
    private const int ReconnectInterval = 5000;

    private WebSocket _ws;
    private Action _onReconnect;

    public WebsocketConnection(String endpointUrl, Action<RoomCommandMessage> onCommandMessage, Action onReconnect)
    {
      _onReconnect = onReconnect;

      _ws = new WebSocket(endpointUrl);
      _ws.MessageReceived += MakeMessageReceived(onCommandMessage);
      _ws.Opened += OnOpened;
      _ws.Closed += OnClosed;
      _ws.Error += OnError;
      _ws.Open();
    }

    public void SendRoomOpenMessage(string roomId)
    {
      var roomConnectionMessage = new RoomConnectionMessage(roomId, RoomStatus.OPEN);
      _ws.Send(JsonConvert.SerializeObject(roomConnectionMessage));
    }

    public void SendRoomClosedMessage(string roomId)
    {
      var roomConnectionMessage = new RoomConnectionMessage(roomId, RoomStatus.CLOSED);
      _ws.Send(JsonConvert.SerializeObject(roomConnectionMessage));
    }

    private EventHandler<MessageReceivedEventArgs> MakeMessageReceived(Action<RoomCommandMessage> onCommandMessage)
    {
      return (sender, args) =>
      {
        var roomCommandMessage = JsonConvert.DeserializeObject<RoomCommandMessage>(args.Message);
        onCommandMessage.Invoke(roomCommandMessage);
      };
    }

    private void OnOpened(object sender, EventArgs e)
    {
      Log.Info("Connection opened: %s", e.ToString());
    }

    private void OnClosed(object sender, EventArgs e)
    {
      Log.Info("Connection closed: %s, attempting reconnect ...", e.ToString());
      Reconnect();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
      Log.Info("Error: %s", e.Exception.Message);
    }

    private void Reconnect()
    {
      var timer = new Timer();

      timer.Interval = ReconnectInterval;
      timer.Elapsed += (sender, args) =>
      {
        switch (_ws.State)
        {
          case WebSocketState.Closed:
            // if the connection is still not open,
            // attempt to open it again
            _ws.Open();
            break;
          case WebSocketState.Open:
            // if a connection exits stop the timer and exit
            timer.Stop();
            timer.Dispose();
            _onReconnect();
            break;
        }

        ;
      };

      timer.Start();
    }
  }
}