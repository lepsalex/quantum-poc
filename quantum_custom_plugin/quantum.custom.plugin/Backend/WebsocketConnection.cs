using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Deterministic;
using Quantum.Model;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;
using WebSocket = WebSocket4Net.WebSocket;
using WebSocketState = System.Net.WebSockets.WebSocketState;

namespace Quantum.Backend
{
  public class WebsocketConnection
  {
    private WebSocket _ws;
    
    public WebsocketConnection(Action<RoomCommandMessage> onCommandMessage)
    {
      _ws = new WebSocket("ws://localhost:8080/ws/commands");
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
      Log.Info("Connection closed: %s", e.ToString());
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
      Log.Info("Error: %s", e.Exception.Message);
    }
  }
}