using System.Collections;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal static class TutorialCombinationLesson
    {
        public static IEnumerator Run(TutorialContext context)
        {
            context.Progress.Step = TutorialStep.CombinationUnavailable;
            context.CraftingBench.gameObject.SetActive(true);
            context.CraftingBench.Close();
            yield return context.ShowDialogue(
                TutorialSpeaker.Guitar,
                "악기 부품을 모두 모으면 서로 어울리는 악기끼리 조합할 수 있어.\nTab 키를 눌러 조합 화면을 열어 봐.",
                "직접 열어 보기");

            context.Dialogue.ShowObjective(
                TutorialSpeaker.Guitar,
                "Tab 키를 눌러 악기 조합 화면을 여세요.");
            yield return new WaitUntil(() => context.CraftingBench.IsVisible);
            context.Dialogue.Hide();

            yield return context.ShowDialogue(
                TutorialSpeaker.Bass,
                "지금은 조합에 쓸 완성된 악기가 없어서 조합할 수 없어.\n연습할 수 있도록 악기 두 개를 준비해 줄게.");

            if (!context.Player.GrantCompletedInstrument(WeaponType.Drum)
                || !context.Player.GrantCompletedInstrument(WeaponType.Bass))
            {
                Debug.LogError("[Tutorial] 조합용 드럼과 베이스를 지급하지 못했습니다.", context.Owner);
                context.CraftingBench.Close();
                context.Player.Movement.UnlockMovement();
                yield break;
            }

            context.CraftingBench.RefreshInventory();
            yield return context.ShowDialogue(
                TutorialSpeaker.Drum,
                "완성된 드럼과 베이스를 지급했어!\n두 악기를 조합 슬롯으로 끌어 놓고 조합 버튼을 눌러 봐.",
                "리듬 섹션 만들기");

            context.Progress.Step = TutorialStep.Combination;
            context.CraftingBench.Open();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Drum,
                "드럼과 베이스를 조합해 리듬 섹션을 완성하세요.");
            yield return new WaitUntil(() =>
                context.Player.Combination.HasCombinedWeapon(CombineWeaponType.RhythmSection));
            context.Dialogue.Hide();
            context.CraftingBench.Close();

            yield return context.ShowDialogue(
                TutorialSpeaker.Bass,
                "리듬 섹션 완성! 주변을 울리는 범위 공격으로\n몰려오는 적들을 한꺼번에 쓸어버려 봐.",
                "합주 시작");

            context.Progress.Step = TutorialStep.CombinedCombat;
            context.Progress.CombinedCombatKillCount = 0;
            context.Player.Movement.UnlockMovement();
            context.Dialogue.ShowObjective(
                TutorialSpeaker.Bass,
                $"리듬 섹션으로 연습용 적 {context.Combat.KillTarget}마리를 쓰러뜨리세요!");
            yield return context.Combat.FightWithCombinedWeapon();

            context.Dialogue.Hide();
            context.Enemies.Clear();
            context.Player.Movement.LockMovement();


            context.Progress.CombinationCompleted = true;
        }
    }
}
