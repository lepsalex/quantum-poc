using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Photon.Hive.Plugin;
using Quantum.CustomState;

namespace Quantum.Backend
{
  public class IntegrationServer
  {
    public const string RootUrl = "http://localhost:8080";
    public const string RoomUrl = RootUrl + "/room";
    public const string AuthUrl = RootUrl + "/auth";
    public const string DemoRoomName = "alex-demo";

    // for demo only, would come from env
    public const string serverAuthToken = "server";

    public HttpRequest RoomSaveRequest(RoomState roomState, HttpRequestCallback onResponseCallback)
    {
      var stream = new MemoryStream();
      var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roomState));
      stream.Write(data, 0, data.Length);

      return new HttpRequest()
      {
        Async = false,
        Url = RoomUrl,
        CustomHeaders = makeAuthHeaders(serverAuthToken),
        Method = "POST",
        ContentType = "application/json",
        DataStream = stream,
        Callback = onResponseCallback
      };
    }

    public HttpRequest RoomRestoreRequest(string userAuthToken, string roomId, HttpRequestCallback onResponseCallback)
    {
      return new HttpRequest()
      {
        Async = false,
        Url = $"{RoomUrl}/{roomId}",
        CustomHeaders = makeAuthHeaders(userAuthToken),
        Method = "GET",
        ContentType = "application/json",
        Callback = onResponseCallback
      };
    }

    public HttpRequest RoomAuthRequest(string userAuthToken, string roomId, HttpRequestCallback onResponseCallback)
    {
      return new HttpRequest()
      {
        Async = false,
        Url = $"{AuthUrl}/room/${roomId}",
        CustomHeaders = makeAuthHeaders(userAuthToken),
        Method = "POST",
        ContentType = "application/json",
        Callback = onResponseCallback
      };
    }

    // demo method only, would be replaced with a real auth header
    private Dictionary<string, string> makeAuthHeaders(string accountId)
    {
      return new Dictionary<string, string>()
      {
        {"X-Fake-Auth", accountId}
      };
    }
  }
}