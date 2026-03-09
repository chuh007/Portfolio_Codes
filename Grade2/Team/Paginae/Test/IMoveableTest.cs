
using UnityEngine;

namespace Work.CUH.Code.Test
{
    // Command 만들때 인터페이스를 이런식으로 분리해두면 의존 쪼개기 좋음.
    // isMoving을 가능한지 검사할때 쓸거니 추가로 빼둠.
    public interface IMoveableTest
    {
        public Transform transform { get; }
        
        void HandleInput(Vector2 input); // 커멘드에서 호출할 함수 하나는 있어야겠죠?
        
        bool isMoving { get; set; }
    }
}