using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Enemies.Boss
{
    public enum PianoBossDialogueTarget
    {
        Player,
        Boss
    }

    [Serializable]
    public sealed class PianoBossDialogueLine
    {
        [SerializeField] private PianoBossDialogueTarget speaker;
        [SerializeField] private PianoBossDialogueTarget focusTarget;
        [SerializeField] private string speakerName;
        [SerializeField, TextArea] private string message;
        [FormerlySerializedAs("showPortrait")]
        [SerializeField] private bool showCharacter = true;

        public PianoBossDialogueTarget Speaker => speaker;
        public PianoBossDialogueTarget FocusTarget => focusTarget;
        public string SpeakerName => speakerName;
        public string Message => message;
        public bool ShowCharacter => showCharacter;
    }

    [CreateAssetMenu(
        fileName = "PianoBossDialogueSequence",
        menuName = "CHUH/Boss/Piano Boss Dialogue Sequence")]
    public sealed class PianoBossDialogueSequenceSO : ScriptableObject
    {
        [SerializeField] private string continueButtonText = "다음";
        [SerializeField] private List<PianoBossDialogueLine> lines = new();

        public string ContinueButtonText => continueButtonText;
        public IReadOnlyList<PianoBossDialogueLine> Lines => lines;
        public bool HasLines => lines is { Count: > 0 };
    }
}
