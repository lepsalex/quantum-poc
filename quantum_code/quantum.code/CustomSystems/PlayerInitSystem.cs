using System;
using System.Collections.Generic;
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

            // link player and set a random color
            if (f.Unsafe.TryGetPointer<PlayerLink>(playerEntity, out var pl))
            {
                pl->PlayerRef = playerRef;
                pl->Color = _playerColors[(playerRef._index - 1) % 6];
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
        
        private Dictionary<int, PlayerColor> _playerColors = new()
        {
            { 0, new PlayerColor{
                R = 130,
                G = 2,
                B = 99,
                A = 255
            }},
            { 1, new PlayerColor{
                R = 217,
                G = 3,
                B = 104,
                A = 255
            }},
            { 2, new PlayerColor{
                R = 234,
                G = 222,
                B = 218,
                A = 255
            }},
            { 3, new PlayerColor{
                R = 46,
                G = 41,
                B = 78,
                A = 255
            }},
            { 4, new PlayerColor{
                R = 255,
                G = 212,
                B = 0,
                A = 255
            }},
            { 5, new PlayerColor{
                R = 173,
                G = 252,
                B = 249,
                A = 255
            }},
        };
    }
}