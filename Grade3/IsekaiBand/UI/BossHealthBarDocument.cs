using System.Threading;
using _Work.CHUH.Code.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.UI
{
    internal class BossHealthBarDocument
    {
        private UIDocument _document;
        private PanelSettings _panelSettings;
        public UIDocument Document => _document;

        public void Dispose()
        {
            if (_panelSettings != null) Object.Destroy(_panelSettings);
        }

        public void CreateDocument(Transform transform, PanelSettings panelSettingsAsset, VisualTreeAsset visualTreeAsset)
        {
            _panelSettings = panelSettingsAsset != null
                ? Object.Instantiate(panelSettingsAsset)
                : ScriptableObject.CreateInstance<PanelSettings>();
            _panelSettings.sortingOrder = 100;

            GameObject documentObject = new GameObject("BossHealthBarDocument");
            documentObject.SetActive(false);
            documentObject.transform.SetParent(transform, false);

            _document = documentObject.AddComponent<UIDocument>();
            _document.panelSettings = _panelSettings;
            _document.visualTreeAsset = visualTreeAsset;

            documentObject.SetActive(true);
        }
    }
}
