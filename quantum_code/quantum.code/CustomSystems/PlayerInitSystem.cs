using System;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class PlayerInitSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        private static Random random = new Random();

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

            // link player and set a random color
            if (f.Unsafe.TryGetPointer<PlayerLink>(playerEntity, out var pl))
            {
                pl->PlayerRef = playerRef;
                pl->Color = GetRandomPlayerColor();
            }
        }

        private bool DoesPlayerExist(Frame f, PlayerRef playerRef)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef == playerRef)
                {
                    return true;
                }
            }

            return false;
        }

        private PlayerColor GetRandomPlayerColor()
        {
            var playerColor = new PlayerColor();

            playerColor.R = GetRandomColorValue();
            playerColor.G = GetRandomColorValue();
            playerColor.B = GetRandomColorValue();
            playerColor.A = 255;

            return playerColor;
        }

        private Int32 GetRandomColorValue()
        {
            // min 50 so that w don't get any colors that are too light or dark
            return random.Next(50, 200);
        }
    }
}