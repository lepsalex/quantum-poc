using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.CustomState.Helpers;

namespace Quantum.CustomState.Commands
{
    public unsafe class CommandRestorePlayerStates : DeterministicCommand
    {
        public List<PlayerState> PlayerStates;

        public override void Serialize(BitStream stream)
        {
            var numPlayers = PlayerStates.Count;
            stream.Serialize(ref numPlayers);

            if (stream.Writing)
            {
                foreach (var player in PlayerStates)
                {
                    var copy = player;
                    stream.Serialize(ref copy.PlayerRef);
                    stream.Serialize(ref copy.PlayerX);
                    stream.Serialize(ref copy.PlayerY);
                    stream.Serialize(ref copy.PlayerZ);
                }
            }
            else
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    PlayerState playerState = default;
                    stream.Serialize(ref playerState.PlayerRef);
                    stream.Serialize(ref playerState.PlayerX);
                    stream.Serialize(ref playerState.PlayerY);
                    stream.Serialize(ref playerState.PlayerZ);
                    PlayerStates.Add(playerState);
                }
            }
        }

        public void Execute(Frame f)
        {
            foreach (var player in PlayerStates)
            {
                var data = f.GetPlayerData(player.PlayerRef);
                var playerPrototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
                var playerEntity = f.Create(playerPrototype);
                
                // update position
                if (f.Unsafe.TryGetPointer<Transform3D>(playerEntity, out var t))
                {
                    t->Position.X = player.PlayerX;
                    t->Position.Y = player.PlayerY;
                    t->Position.Z = player.PlayerZ;
                }

                // link player and set a color
                if (f.Unsafe.TryGetPointer<PlayerLink>(playerEntity, out var pl))
                {
                    pl->PlayerRef = player.PlayerRef;
                    pl->Color = PlayerColorHelper.GetPlayerColor(player.PlayerRef);
                }
            }
        }
    }
}