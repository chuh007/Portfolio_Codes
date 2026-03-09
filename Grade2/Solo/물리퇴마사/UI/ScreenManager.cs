using _01Scripts.Core.EventSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.UI
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO systemChannel;
        
        private readonly int _circleSizeHash = Shader.PropertyToID("_CircleSize");

        private void Awake()
        {
            //UI의 이미지는 스프라이트 렌더러와 다르게 모두가 같은 매티리얼을 참조해서 쓴다.
            //그래서 이렇게 안해주면 같은 메티리얼을 쓰는 모든 애들이 다 똑같이 동작하게 된다.
            fadeImage.material = new Material(fadeImage.material);
            uiChannel.AddListener<FadeEvent>(HandleFadeScreen);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<FadeEvent>(HandleFadeScreen);
        }

        private void HandleFadeScreen(FadeEvent fadeEvt)
        {
            float fadeValue = fadeEvt.isFadeIn ? 2f : 0;
            float startValue = fadeEvt.isFadeIn ? 0f : 2f;
            
            fadeImage.material.SetFloat(_circleSizeHash, startValue);

            if (fadeEvt.isFadeIn) //새로운 씬에 들어온거니까.
            {
                LoadGameEvent loadEvt = SystemEvents.LoadGameEvent;
                loadEvt.isLoadFromFile = true;
                systemChannel.RaiseEvent(loadEvt);
            }
            
            fadeImage.material.DOFloat(fadeValue, _circleSizeHash, fadeEvt.fadeTime).OnComplete(() =>
            {
                if (fadeEvt.isFadeIn == false) //씬을 이동하기 위해 암전한 상태
                {
                    SaveGameEvent saveEvt = SystemEvents.SaveGameEvent;
                    saveEvt.isSaveToFile = true;
                    systemChannel.RaiseEvent(saveEvt);
                }
                uiChannel.RaiseEvent(UIEvents.FadeCompleteEvent);
            });
            
        }
    }
    
}