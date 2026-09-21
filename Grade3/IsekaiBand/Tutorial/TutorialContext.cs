using System.Collections;
using _Work.CHUH.Code.UI;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialContext
    {
        public TutorialSequenceController Owner { get; }
        public TutorialDialogueView Dialogue => Owner.Dialogue;
        public CraftingBenchToolkitController CraftingBench => Owner.CraftingBench;
        public TutorialPlayerContext Player { get; }
        public TutorialTrainingEnemies Enemies { get; }
        public TutorialCombatPractice Combat { get; }
        public TutorialProgress Progress { get; }
        public string CompletionSceneName => Owner.CompletionSceneName;

        public TutorialContext(TutorialSequenceController owner, TutorialPlayerContext player,
            TutorialTrainingEnemies enemies, TutorialCombatPractice combat, TutorialProgress progress)
        {
            Owner = owner;
            Player = player;
            Enemies = enemies;
            Combat = combat;
            Progress = progress;
        }

        public bool Validate(bool hasPianoCharacter)
        {
            if (Dialogue != null && CraftingBench != null && Player.IsReady && Enemies.IsReady && hasPianoCharacter)
                return true;
            Debug.LogError("[Tutorial] 필수 참조가 없어 튜토리얼을 시작할 수 없습니다.", Owner);
            Player.Movement?.UnlockMovement();
            return false;
        }

        public IEnumerator ShowDialogue(TutorialSpeaker speaker, string message, string buttonText = "다음")
        {
            Dialogue.ShowDialogue(speaker, message, buttonText, blockGameplayUi: false);
            yield return new WaitUntil(() => Dialogue.AdvanceRequested);
            Dialogue.Hide();
        }
    }
}
