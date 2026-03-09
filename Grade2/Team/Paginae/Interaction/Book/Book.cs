using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.SO;

namespace Work.CUH.Code.Interaction.Book
{
    public class Book : MonoBehaviour, IInteraction
    {
        private static readonly int FadeValue = Shader.PropertyToID("_FadeValue");
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private string loadSceneName;
        private Animator _animator;
        
        [SerializeField] private StageInfoSO stageInfo;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            Bus<CloseBookUIEvent>.OnEvent -= HandleCloseBook;
        }
        
        public void Interact()
        {
            _animator.SetBool(Open, true);
        }

        private void OpenBook()
        {
            Bus<OpenBookUIEvent>.Raise(new OpenBookUIEvent(loadSceneName, stageInfo));
            Bus<CloseBookUIEvent>.OnEvent += HandleCloseBook;
        }
        
        
        private void HandleCloseBook(CloseBookUIEvent evt)
        {
            _animator.SetBool(Open, false);
            Bus<CloseBookUIEvent>.OnEvent -= HandleCloseBook;
        }
    }
}