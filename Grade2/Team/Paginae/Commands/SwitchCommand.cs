using Work.CUH.Code.Command;
using Work.CUH.Code.SwitchSystem;

namespace Work.CUH.Code.Commands
{
    public class SwitchCommand : BaseCommand
    {
        public SwitchCommand(ICommandable commandable) : base(commandable)
        {
            
        }

        public override bool CanExecute()
        {
            return Commandable is ISwitch;
        }

        public override void Execute()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.ToggleSwitch();
        }

        public override void Undo()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.UndoSwitch();
        }
    }
}