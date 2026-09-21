using System.Collections;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialMovementLesson
    {
        public static IEnumerator Run(TutorialContext context)
        {
            context.Progress.Step = TutorialStep.Movement;
            context.Player.Movement.UnlockMovement();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Vocal,
                "WASD 키 또는 마우스 드래그!\n둘 중 편한 방식으로 움직여 봐.");
            yield return context.Player.WaitForMovement();
            yield return new WaitUntil(() => !context.Player.IsDashing);

            context.Player.Movement.LockMovement();
            yield return context.ShowDialogue(
                TutorialSpeaker.Drum,
                "좋아, 이동은 완벽해!\n움직이는 중에 Shift 키를 누르면 대시해서 위험한 순간을 빠르게 빠져나갈 수 있어.");

            context.Progress.Step = TutorialStep.Dash;
            context.Progress.DashPerformed = false;
            context.Player.Movement.UnlockMovement();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Drum,
                "방향을 잡고 이동하면서 Shift 키를 눌러 대시해 봐!");
            // 시작 이벤트만으로 넘어가면 설명창의 입력 잠금이 진행 중인 대시를 끊는다.
            yield return new WaitUntil(() => context.Progress.DashPerformed && !context.Player.IsDashing);

            context.Player.Movement.LockMovement();
            yield return context.ShowDialogue(
                TutorialSpeaker.Piano,
                "바로 그거야! 이제 피아노로 연주해 볼까?\n공격은 자동이야. 적 가까이 가면 음표가 날아가 공격해.");
        }
    }
}
