namespace Quantum.Model
{
  public class RoomConnectionMessage
  {
    public string RoomId { get; }
    public RoomStatus RoomStatus { get; }

    public RoomConnectionMessage(string roomId, RoomStatus roomStatus)
    {
      RoomId = roomId;
      RoomStatus = roomStatus;
    }
  }
  
  public enum RoomStatus {
    OPEN,
    CLOSED
  }
}