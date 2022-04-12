namespace Quantum
{
    public unsafe class PlayerDisconnectSystem : SystemSignalsOnly, ISignalOnPlayerDisconnected
    {
        public void OnPlayerDisconnected(Frame f, PlayerRef playerRef)
        {
            Log.Info($"Player {playerRef} has disconnected!");
            
            // // Below code is example of how to remove player character
            // // from scene on disconnect
            // foreach (var player in f.GetComponentIterator<PlayerLink>())
            // {
            //
            //     if (player.Component.PlayerRef == playerRef)
            //     {
            //         f.Destroy(player.Entity);
            //         Log.Debug($"Player {playerRef} has disconnected, entity destroyed!");
            //     }
            // }
        }
    }
}