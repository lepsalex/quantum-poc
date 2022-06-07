using Photon.Deterministic;

namespace Quantum.CustomState.Commands
{
    public class CommandRemovePlayer : DeterministicCommand
    {
        public int Player;  
        
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Player);
        }
        
        public void Execute(Frame f)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef._index == Player)
                {
                    f.Destroy(player.Entity);
                }
            }
        }
    }
}