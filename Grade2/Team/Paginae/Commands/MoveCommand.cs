using UnityEngine;
using Work.CIW.Code.Player;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Command;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.Test;

namespace Work.CUH.Code.Commands
{
    // 주의사항 : 커멘드를 쓸때는 반드시 생성해서 써야한다. 안 그러면 꼬임.
    public class MoveCommand : BaseCommand
    {
        public MoveCommand(ICommandable commandable, Vector2 dir) : base(commandable)
        {
            Dir = dir;
        }

        public Vector2 Dir { get; set; }
        
        public override bool CanExecute()
        {
            // 이 부분은 커멘드마다 달라짐. 적당히 조건 맞춰야한다.
            // 이거같은 경우는 IMoveable해야 하고, 그놈이 움직이고 있지 않아야 작동 가능하다.
            return Commandable is IMoveableTest moveable
                && moveable.isMoving == false;
        }
        
        // 실행이나 언도는 내부구현 적당히 하면 됨. (이래서 인터페이스 분리가 중요. 안하면 강한의존)
        public override void Execute()
        {
            IMoveableTest movement = Commandable as IMoveableTest;
            movement.HandleInput(Dir);
            
            if (Commandable is IMovement)
                Bus<PlayerPosChangeEvent>.Raise(
                    new PlayerPosChangeEvent(movement.transform.position, new Vector3(Dir.x, 0, Dir.y)));
            else
                Bus<TargetPosChangeEvent>.Raise(
                    new TargetPosChangeEvent(movement.transform, new Vector3(Dir.x, 0, Dir.y)));
        }
        
        public override void Undo()
        {
            IMoveableTest movement = Commandable as IMoveableTest;
            movement.HandleInput(-Dir);
        }
    }
}