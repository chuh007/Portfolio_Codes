using UnityEngine;
using Work.CUH.Code.Command;
using Work.PSB.Code.Player;

namespace Work.CUH.Code.Commands
{
    public class StairCommand : BaseCommand
    {
        public StairCommand(ICommandable commandable, Vector3Int beforePos, Vector3Int afterPos, Vector3Int dir) : base(commandable)
        {
            BeforePos = beforePos;
            AfterPos = afterPos;
            Dir = dir;
        }

        public Vector3Int BeforePos { get; private set; }
        public Vector3Int AfterPos { get; private set; }
        public Vector3Int Dir { get; private set; }
        

        public override bool CanExecute()
        {
            return Commandable is PSBTestPlayerMovement;
        }

        public override void Execute()
        {
            var movement = Commandable as PSBTestPlayerMovement;
            movement.TeleportToFloor(AfterPos, Dir);
        }

        public override void Undo()
        {
            var movement = Commandable as PSBTestPlayerMovement;
            Debug.Log($"언도에서 보내줄 DIr : {Dir}");
            movement.TeleportToFloor(BeforePos, Dir);
        }
    }
}