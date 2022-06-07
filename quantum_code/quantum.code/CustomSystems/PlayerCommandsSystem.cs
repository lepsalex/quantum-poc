using Quantum.CustomState.Commands;

namespace Quantum
{
    public class PlayerCommandsSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int i = 0; i < f.PlayerCount; i++)
            {
                var command1 = f.GetPlayerCommand(i) as CommandRestorePlayerState;
                command1?.Execute(f);
                
                var command2 = f.GetPlayerCommand(i) as CommandChangePlayerColor;
                command2?.Execute(f);            
                
                var command3 = f.GetPlayerCommand(i) as CommandRemovePlayer;
                command3?.Execute(f);
            }
        }
    }
}