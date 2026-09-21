using System.Threading;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.UI;
using _Work.CHUH.Code.Tutorial;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal static class PianoBossDialogueSequencePlayer
    {
        public static async UniTask<bool> PlayAsync(
            PianoBossDialogueSequenceSO sequence,
            TutorialDialogueView dialogueView,
            Transform playerTarget,
            Transform bossTarget,
            DialogueCameraFocus cameraFocus,
            CancellationToken cancellationToken)
        {
            if (sequence == null || dialogueView == null || !sequence.HasLines)
                return false;

            foreach (PianoBossDialogueLine line in sequence.Lines)
            {
                if (line == null)
                    continue;

                dialogueView.Hide();
                Transform focusTarget = ResolveTarget(
                    line.FocusTarget,
                    playerTarget,
                    bossTarget);
                bool canceled = await cameraFocus.FocusAsync(
                    focusTarget,
                    cancellationToken);
                if (canceled)
                    return true;

                Transform portraitTarget = ResolveTarget(
                    line.Speaker,
                    playerTarget,
                    bossTarget);
                dialogueView.ShowBottomDialogue(
                    ResolveSpeakerName(line),
                    ResolveCharacterSprite(
                        line.Speaker,
                        portraitTarget),
                    line.Message,
                    sequence.ContinueButtonText,
                    line.ShowCharacter,
                    line.Speaker == PianoBossDialogueTarget.Player
                        ? DialogueCharacterSide.Left
                        : DialogueCharacterSide.Right);

                canceled = await UniTask.WaitUntil(
                        () => dialogueView.AdvanceRequested,
                        cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                    return true;
            }

            dialogueView.Hide();
            return false;
        }

        private static Transform ResolveTarget(
            PianoBossDialogueTarget target,
            Transform playerTarget,
            Transform bossTarget)
        {
            return target == PianoBossDialogueTarget.Player
                ? playerTarget
                : bossTarget;
        }

        private static string ResolveSpeakerName(PianoBossDialogueLine line)
        {
            if (!string.IsNullOrWhiteSpace(line.SpeakerName))
                return line.SpeakerName;

            return line.Speaker == PianoBossDialogueTarget.Player
                ? "플레이어"
                : "피아니스트";
        }

        private static Sprite ResolveCharacterSprite(
            PianoBossDialogueTarget speaker,
            Transform target)
        {
            if (target == null)
                return null;

            if (speaker == PianoBossDialogueTarget.Player)
            {
                PlayerAttackCompo attackCompo =
                    target.GetComponentInParent<PlayerAttackCompo>();
                PlayerCharacterDataSO startingWeapon =
                    attackCompo?.StartingWeaponData
                    ?? GameStartSelectionContext.SelectedCharacterData;
                if (startingWeapon != null && startingWeapon.dialogueSprite != null)
                    return startingWeapon.dialogueSprite;
            }

            Entity entity = target.GetComponent<Entity>();
            EntityRenderer entityRenderer = entity?.GetCompo<EntityRenderer>();
            SpriteRenderer spriteRenderer = entityRenderer != null
                ? entityRenderer.GetComponent<SpriteRenderer>()
                  ?? entityRenderer.GetComponentInChildren<SpriteRenderer>(true)
                : target.GetComponentInChildren<SpriteRenderer>(true);
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }
    }
}
