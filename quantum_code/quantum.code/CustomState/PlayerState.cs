using System;
using Photon.Deterministic;

namespace Quantum.CustomState
{
    [Serializable]
    public struct PlayerState
    {
        public PlayerRef PlayerRef;
        public AssetRefEntityPrototype PlayerPrototype;
        public FP PlayerX;
        public FP PlayerY;
        public FP PlayerZ;
    }
}