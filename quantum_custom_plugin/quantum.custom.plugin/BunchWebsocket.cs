using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Photon.Deterministic;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;
using WebSocket = WebSocket4Net.WebSocket;
using WebSocketState = System.Net.WebSockets.WebSocketState;

namespace Quantum
{
  public class BunchWebsocket
  {
    private WebSocket _ws;


    public BunchWebsocket()
    {
      _ws = new WebSocket("ws://localhost:8080/ws/commands");
      _ws.MessageReceived += MessageReceived;
      _ws.Opened += OnOpened;
      _ws.Closed += OnClosed;
      _ws.Error += OnError;
      _ws.Open();
    }

    public void Send(string message)
    {
      _ws.Send(message);
    }
    
    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
      Log.Info("Message received: %s", e.Message);
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