using System;
using Photon.Deterministic;

namespace Quantum.CustomState
{
    [Serializable]
    public class PlayerState
    {
        public PlayerRef PlayerRef;
        public FP PlayerX;
        public FP PlayerY;
        public FP PlayerZ;
    }
}