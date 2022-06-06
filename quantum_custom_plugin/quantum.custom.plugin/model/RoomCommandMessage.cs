using System;
using System.Collections.Generic;

namespace Quantum.model
{
  public class RoomCommandMessage
  {
    public string RoomId;
    public string Type;
    public Dictionary<string, Object> Data;

    public RoomCommandMessage(string roomId, string type, Dictionary<string, object> data)
    {
      RoomId = roomId;
      Type = type;
      Data = data;
    }
  }
}