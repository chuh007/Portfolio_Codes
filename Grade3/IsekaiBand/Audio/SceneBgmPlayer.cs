using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    [DisallowMultipleComponent]
    public sealed class SceneBgmPlayer : MonoBehaviour
    {
        [SerializeField] private string soundKey;

        private void Start()
        {
            if (string.IsNullOrWhiteSpace(soundKey))
            {
                Debug.LogWarning("[SceneBgmPlayer] BGM 사운드 키가 비어 있습니다.", this);
                return;
            }

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(soundKey, SoundType.BGM));
        }
    }
}
