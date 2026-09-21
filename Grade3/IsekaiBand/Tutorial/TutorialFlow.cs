using System.Collections;
using _Code.LCH._02.Scripts.Bus;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialFlow
    {
        public static IEnumerator Run(TutorialContext context)
        {
            context.Progress.Step = TutorialStep.Introduction;
            yield return context.ShowDialogue(
                TutorialSpeaker.Piano,
                "어서 와! 나는 피아노 파트를 맡고 있어.\n짧게 몸을 풀면서 전투의 기본부터 익혀 보자.");


            yield return TutorialMovementLesson.Run(context);
            yield return TutorialCombatLesson.Run(context);
            yield return TutorialCombinationLesson.Run(context);
            if (!context.Progress.CombinationCompleted) yield break;

            context.Progress.Step = TutorialStep.Complete;
            yield return context.ShowDialogue(
                TutorialSpeaker.Vocal,
                "악기 조합과 합주까지 완벽해! 이제 준비는 끝났어.\n무대에서도 지금처럼 움직이고, 피하고, 연주하면 돼!",
                "튜토리얼 완료");

            Time.timeScale = 1f;
            Bus<FadeEvent>.Raise(new FadeEvent(context.CompletionSceneName));
        }
    }
}
