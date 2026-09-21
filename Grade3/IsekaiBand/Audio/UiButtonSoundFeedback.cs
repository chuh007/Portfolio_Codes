using System.Collections;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UguiButton = UnityEngine.UI.Button;

namespace _Work.CHUH.Code.Audio
{
    /// <summary>씬과 런타임에서 생성되는 버튼에 공통 클릭음을 연결한다.</summary>
    public sealed class UiButtonSoundFeedback : MonoBehaviour
    {
        private const float BindingRefreshInterval = 0.5f;
        private static UiButtonSoundFeedback _instance;
        private readonly UguiButtonSoundBinding _ugui = new(PlayButtonClick);
        private readonly ToolkitButtonSoundBinding _toolkit = new(PlayButtonClick);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState() => _instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() => EnsureInstance();

        private static UiButtonSoundFeedback EnsureInstance()
        {
            if (_instance != null) return _instance;

            UiButtonSoundFeedback existing = FindFirstObjectByType<UiButtonSoundFeedback>();
            if (existing != null)
            {
                _instance = existing;
                return existing;
            }

            var feedbackObject = new GameObject(nameof(UiButtonSoundFeedback));
            return feedbackObject.AddComponent<UiButtonSoundFeedback>();
        }

        public static void ExcludeUguiButton(UguiButton button)
        {
            if (button != null)
                EnsureInstance()._ugui.Exclude(button);
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
        }

        private void OnEnable()
        {
            if (_instance != this) return;

            SceneManager.sceneLoaded += HandleSceneLoaded;
            StartCoroutine(RefreshBindingsLoop());
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            StopAllCoroutines();
            _ugui.Clear();
            _toolkit.Clear();
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
            => StartCoroutine(RefreshAfterSceneSetup());

        private IEnumerator RefreshAfterSceneSetup()
        {
            yield return null;
            RefreshBindings();
        }

        private IEnumerator RefreshBindingsLoop()
        {
            var wait = new WaitForSecondsRealtime(BindingRefreshInterval);
            while (enabled)
            {
                RefreshBindings();
                yield return wait;
            }
        }

        private void RefreshBindings()
        {
            _ugui.Refresh();
            _toolkit.Refresh();
        }

        private static void PlayButtonClick()
            => Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.UiButtonClick, SoundType.SFX));
    }
}
