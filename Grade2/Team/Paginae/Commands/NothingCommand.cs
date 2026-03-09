using Work.CUH.Code.Command;

namespace Work.CUH.Code.Commands
{
    public class NothingCommand : BaseCommand
    {
        public NothingCommand(ICommandable commandable) : base(commandable)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            
        }

        public override void Undo()
        {
            
        }
    }
}