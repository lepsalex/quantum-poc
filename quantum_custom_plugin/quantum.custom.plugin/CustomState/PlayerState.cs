namespace Quantum.CustomState
{
  public readonly struct PlayerState
  {
    public PlayerState(PlayerLink playerLink, Transform3D playerTransform3D)
    {
      PlayerLink = playerLink;
      PlayerTransform3D = playerTransform3D;
    }

    public PlayerLink PlayerLink { get; }
    public Transform3D PlayerTransform3D { get; }
  }
}