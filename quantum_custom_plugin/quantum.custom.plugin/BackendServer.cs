using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using Photon.Deterministic.Server;
using Photon.Hive.Plugin;
using Quantum.CustomState;
using Quantum.CustomState.Commands;

namespace Quantum
{
  public class BackendServer
  {
    public const string RootUrl = "http://localhost:8080";
    public const string RoomUrl = RootUrl + "/room";
    public const string DemoRoomName = "alex-demo";

    public HttpRequest roomSaveRequest(RoomState roomState)
    {
      var stream = new MemoryStream();
      var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roomState));
      stream.Write(data, 0, data.Length);

      return new HttpRequest()
      {
        Async = false,
        Url = RoomUrl,
        Method = "POST",
        ContentType = "application/json",
        DataStream = stream
      };
    }

    public HttpRequest roomRestoreRequest(string roomId, HttpRequestCallback onResponseCallback)
    {
      return new HttpRequest()
      {
        Async = false,
        Url = $"{RoomUrl}/{roomId}",
        Method = "GET",
        ContentType = "application/json",
        Callback = onResponseCallback
      };
    }
  }
}