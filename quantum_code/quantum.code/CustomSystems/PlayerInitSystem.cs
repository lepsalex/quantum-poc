using Photon.Deterministic;

namespace Quantum
{
    public unsafe class PlayerInitSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
        {
            if (DoesPlayerExist(f, playerRef)) return;
            
            var data = f.GetPlayerData(playerRef);
            var playerPrototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var playerEntity = f.Create(playerPrototype);

            // update X position
            if (f.Unsafe.TryGetPointer<Transform3D>(playerEntity, out var t))
            {
                t->Position.X = 0 + playerRef;
            }

            // link player
            if (f.Unsafe.TryGetPointer<PlayerLink>(playerEntity, out var pl))
            {
                pl->PlayerRef = playerRef;
            }
        }
        
        private bool DoesPlayerExist(Frame f, PlayerRef playerRef)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef == playerRef) {
                    return true;
                }
            }
            return false;
        }
    }
}