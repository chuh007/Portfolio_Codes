using System.Collections;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialCombatLesson
    {
        public static IEnumerator Run(TutorialContext context)
        {
            context.Progress.Step = TutorialStep.FirstCombat;
            context.Player.Movement.UnlockMovement();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Piano,
                "피아노의 자동 공격으로 연습용 적 한 마리를 쓰러뜨려 봐!");
            yield return context.Combat.FightFirstEnemy();

            context.Player.Movement.LockMovement();
            yield return context.ShowDialogue(
                TutorialSpeaker.Bass,
                "멋진 연주였어! 적이 떨어뜨린 경험치 음표를 주우면 성장할 수 있어.\n계속 싸워서 레벨 업해 보자!");

            context.Progress.Step = TutorialStep.LevelUpCombat;
            context.Player.Movement.UnlockMovement();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Bass,
                "적을 계속 쓰러뜨리고 경험치 음표를 모아 레벨 업하세요!");
            yield return context.Combat.WaitForLevelUp();

            context.Enemies.Clear();
            context.Player.Movement.LockMovement();
            yield return context.ShowDialogue(
                TutorialSpeaker.Guitar,
                "레벨 업! 화면에 나온 3개의 강화 선택지 중\n마음에 드는 하나를 골라 봐.",
                "선택지 보기");

            context.Progress.Step = TutorialStep.CardSelection;
            context.Dialogue.Hide();
            yield return new WaitUntil(() => context.Progress.CardSelected);
            yield return null;
        }
    }
}
