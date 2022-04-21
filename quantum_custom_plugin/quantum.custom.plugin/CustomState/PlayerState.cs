using Photon.Deterministic;

namespace Quantum.CustomState
{
  public readonly struct PlayerState
  {
    public PlayerState(PlayerLink playerLink, FP playerX, FP playerY, FP playerZ)
    {
      PlayerLink = playerLink;
      PlayerX = playerX;
      PlayerY = playerY;
      PlayerZ = playerZ;
    }

    public PlayerLink PlayerLink { get; }
    public FP PlayerX { get; }
    public FP PlayerY { get; }
    public FP PlayerZ { get; }
  }
}