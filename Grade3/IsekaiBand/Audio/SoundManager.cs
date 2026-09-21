using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    public sealed class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        private SoundPlaybackController _playback;

        public static SoundManager Instance => EnsureInstance();
        public static bool HasInstance => _instance != null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState() => _instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() => EnsureInstance();

        private static SoundManager EnsureInstance()
        {
            if (_instance != null) return _instance;

            SoundManager existing = FindFirstObjectByType<SoundManager>();
            if (existing != null)
            {
                _instance = existing;
                return existing;
            }

            var managerObject = new GameObject(nameof(SoundManager));
            return managerObject.AddComponent<SoundManager>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            _playback = new SoundPlaybackController(transform);
        }

        private void OnEnable()
        {
            if (_instance == this)
                _playback.Subscribe();
        }

        private void OnDisable() => _playback?.Unsubscribe();

        private void LateUpdate() => _playback?.Bgm.Tick(Time.unscaledDeltaTime);

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static void SetMasterVolume(float volume)
            => EnsureInstance()._playback.Volumes.SetMasterVolume(volume);

        public static void SetSfxVolume(float volume)
            => EnsureInstance()._playback.Volumes.SetSfxVolume(volume);

        public static void SetBgmVolume(float volume)
            => EnsureInstance()._playback.Volumes.SetBgmVolume(volume);

        public static void StopBgm() => EnsureInstance()._playback.Bgm.Stop();

        public static UniTask<bool> FadeOutBgmAsync(float duration, CancellationToken cancellationToken)
            => EnsureInstance()._playback.Bgm.FadeOutAsync(duration, cancellationToken);
    }
}
