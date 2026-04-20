using UnityEngine;

namespace A.Work.CHUH._01.Script.UI.PopUp
{
    public interface IPopupable
    {
        public GameObject gameObject { get; }
        
        public void PopUp()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}