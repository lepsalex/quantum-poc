using System;
using System.Collections.Generic;

namespace Quantum.CustomState
{
  public readonly struct RoomState
  {
    public RoomState(string roomName, List<PlayerState> playerStateList)
    {
      RoomName = roomName;
      PlayerStateList = playerStateList;
    }

    public String RoomName { get; }
    public List<PlayerState> PlayerStateList { get; }
  }
}