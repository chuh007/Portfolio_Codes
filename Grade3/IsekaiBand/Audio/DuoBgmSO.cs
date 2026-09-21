using System;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    [Serializable]
    public struct DuoBgmEntry
    {
        public CombineWeaponType combination;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [CreateAssetMenu(fileName = "DuoBgm", menuName = "SO/Audio/Duo BGM")]
    public class DuoBgmSO : ScriptableObject
    {
        [SerializeField, Min(0f)] private float transitionDuration = 0.35f;
        [SerializeField] private DuoBgmEntry[] entries = Array.Empty<DuoBgmEntry>();

        public float TransitionDuration => Mathf.Max(0f, transitionDuration);

        public bool TryGet(CombineWeaponType combination, out DuoBgmEntry entry)
        {
            foreach (DuoBgmEntry candidate in entries)
            {
                if (candidate.combination != combination || candidate.clip == null) continue;
                entry = candidate;
                return true;
            }

            entry = default;
            return false;
        }
    }
}
