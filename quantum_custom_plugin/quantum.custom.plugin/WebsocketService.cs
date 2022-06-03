using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
  public class BunchWebsocket
  {
    public static string Connection = "ws://localhost:8080/ws/commands";
    public static CancellationTokenSource Cts = new CancellationTokenSource();
    public static ClientWebSocket Socket = new ClientWebSocket();

    public static async System.Threading.Tasks.Task Connect()
    {
      try
      {
        await Socket.ConnectAsync(new Uri(Connection), Cts.Token);
      } catch (Exception ex)
      {
        Console.WriteLine($"ERROR - {ex.Message}");
      }
    }

    public static async System.Threading.Tasks.Task Send(string data)
    {
      if (!Socket.State.Equals(WebSocketState.Open))
      {
        await Connect();
      }

      await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true,
        Cts.Token);
    }

    public static async System.Threading.Tasks.Task Receive(Action<string> onMessage)
    {
      if (!Socket.State.Equals(WebSocketState.Open))
      {
        await Connect();
      }

      await System.Threading.Tasks.Task.Factory.StartNew(
        async () =>
        {
          var rcvBytes = new byte[128];
          var rcvBuffer = new ArraySegment<byte>(rcvBytes);
          while (true)
          {
            WebSocketReceiveResult rcvResult = await Socket.ReceiveAsync(rcvBuffer, Cts.Token);
            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
            string rcvMsg = Encoding.UTF8.GetString(msgBytes);
            onMessage.Invoke(rcvMsg);
          }
        }, Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
  }
}