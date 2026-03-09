using UnityEngine;
using Work.ISC.Code.UI;

namespace Work.CUH.Code.UI
{
    public class FillLibraryBtn : AbstractFillButtonUI
    {
        [SerializeField] private GameObject libraryTransition;
        protected override void HandleBtnClick()
        {
            libraryTransition.SetActive(true);
        }
    }
}