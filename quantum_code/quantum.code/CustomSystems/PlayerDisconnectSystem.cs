namespace Quantum
{
    public unsafe class PlayerDisconnectSystem : SystemSignalsOnly, ISignalOnPlayerDisconnected
    {
        public void OnPlayerDisconnected(Frame f, PlayerRef playerRef)
        {
            
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef == playerRef)
                {
                    f.Destroy(player.Entity);
                    Log.Debug($"Player {playerRef} has disconnected, entity destroyed!");
                }
            }
        }
    }
}