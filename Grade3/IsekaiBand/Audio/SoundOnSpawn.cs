using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    [DisallowMultipleComponent]
    public sealed class SoundOnSpawn : MonoBehaviour
    {
        [SerializeField] private string soundKey;
        [SerializeField, Min(0f)] private float volumeMultiplier = 1f;
        [SerializeField] private bool suppressDuplicateThisFrame;
        [SerializeField, Min(0f)] private float duplicateSuppressionWindowSeconds;

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(soundKey))
                return;

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                soundKey,
                SoundType.SFX,
                volumeMultiplier,
                suppressDuplicateThisFrame: suppressDuplicateThisFrame,
                duplicateSuppressionWindowSeconds: duplicateSuppressionWindowSeconds));
        }
    }
}
