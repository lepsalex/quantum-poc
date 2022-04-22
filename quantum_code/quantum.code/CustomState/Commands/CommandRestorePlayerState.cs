using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.CustomState.Helpers;

namespace Quantum.CustomState.Commands
{
    public unsafe class CommandRestorePlayerState : DeterministicCommand
    {
        public PlayerRef PlayerRef;
        public AssetRefEntityPrototype PlayerPrototype;
        public FP PlayerX;
        public FP PlayerY;
        public FP PlayerZ;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref PlayerRef);
            stream.Serialize(ref PlayerPrototype);
            stream.Serialize(ref PlayerX);
            stream.Serialize(ref PlayerY);
            stream.Serialize(ref PlayerZ);
        }

        public void Execute(Frame f)
        {
            var playerEntity = f.Create(PlayerPrototype);

            // update position
            if (f.Unsafe.TryGetPointer<Transform3D>(playerEntity, out var t))
            {
                t->Position.X = PlayerX;
                t->Position.Y = PlayerY;
                t->Position.Z = PlayerZ;
            }

            // link player and set a random color
            if (f.Unsafe.TryGetPointer<PlayerLink>(playerEntity, out var pl))
            {
                pl->PlayerRef = PlayerRef;
                pl->Color = PlayerColorHelper.GetPlayerColor(PlayerRef);
            }
        }
    }
}