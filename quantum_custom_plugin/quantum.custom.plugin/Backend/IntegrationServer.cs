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
    private readonly string _roomUrl;
    private readonly string _authUrl;
    private readonly string _serverAuthToken = "server";

    public const string DemoRoomName = "alex-demo";

    public IntegrationServer(string baseUrl, string serverAuthToken)
    {
      _roomUrl = baseUrl + "/room";
      _authUrl = baseUrl + "/auth";
      _serverAuthToken = serverAuthToken;
    }

    public HttpRequest RoomSaveRequest(RoomState roomState, HttpRequestCallback onResponseCallback)
    {
      var stream = new MemoryStream();
      var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(roomState));
      stream.Write(data, 0, data.Length);

      return new HttpRequest()
      {
        Async = false,
        Url = _roomUrl,
        CustomHeaders = makeAuthHeaders(_serverAuthToken),
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
        Url = $"{_roomUrl}/{roomId}",
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
        Url = $"{_authUrl}/room/${roomId}",
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