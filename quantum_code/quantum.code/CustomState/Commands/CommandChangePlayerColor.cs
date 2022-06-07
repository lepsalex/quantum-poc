using Photon.Deterministic;
using Quantum.CustomState.Helpers;

namespace Quantum.CustomState.Commands
{
    public unsafe class CommandChangePlayerColor : DeterministicCommand
    {
        public int Player;
        public int NewColorIndex;
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Player);
            stream.Serialize(ref NewColorIndex);
        }

        public void Execute(Frame f)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef._index == Player)
                {
                    if (f.Unsafe.TryGetPointer<PlayerLink>(player.Entity, out var pl))
                    {
                        pl->Color = PlayerColorHelper.GetColorAtIndex(NewColorIndex);
                        f.Events.PlayerDataUpdate(pl->PlayerRef);
                    }
                }
            }
        }
    }
}