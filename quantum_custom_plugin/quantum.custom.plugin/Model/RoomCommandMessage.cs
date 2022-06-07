using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.CustomState.Commands;

namespace Quantum.Model
{
  public class RoomCommandMessage
  {
    private const string CommandChangePlayerColor = "CommandChangePlayerColor";
    private const string CommandRemovePlayer = "CommandRemovePlayer";

    public string RoomId;
    public string Type;
    public Dictionary<string, Object> Data;

    public RoomCommandMessage(string roomId, string type, Dictionary<string, object> data)
    {
      RoomId = roomId;
      Type = type;
      Data = data;
    }

    /**
     * Converts a RoomCommandMessage to DeterministicCommand if
     * we have a mapping for the type, else it will be null
     */
    public bool TryGetDeterministicCommand(out DeterministicCommand command)
    {
      switch (Type)
      {
        case CommandChangePlayerColor:
          command = ToCommandChangePlayerColor();
          return true;
        case CommandRemovePlayer:
          command = ToCommandRemovePlayer();
          return true;
        default:
          command = null;
          return false;
      }
    }

    private CommandChangePlayerColor ToCommandChangePlayerColor()
    {
      return new CommandChangePlayerColor()
      {
        Player = Convert.ToInt32(Data["Player"]),
        NewColorIndex = Convert.ToInt32(Data["NewColorIndex"])
      };
    }

    private CommandRemovePlayer ToCommandRemovePlayer()
    {
      return new CommandRemovePlayer()
      {
        Player = Convert.ToInt32(Data["Player"])
      };
    }
  }
}