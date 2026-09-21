#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Card;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    public class LevelUpCardsDropInTestObject : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private List<UpgradeCardBaseDataSO> previewCards = new();
        [SerializeField, Min(0)] private int sortingOrder = 200;
        [SerializeField] private bool createOnAwake = true;

        private PanelSettings _panelSettings;
        private LevelUpCardsDropInPlayer _player;

        private void Awake()
        {
#if UNITY_EDITOR
            FillDefaultAssets();
#endif
            if (createOnAwake)
                Create();
        }

        public void Create()
        {
            if (_player != null)
                return;

            if (visualTreeAsset == null)
            {
                Debug.LogError($"{nameof(LevelUpCardsDropInTestObject)} needs a VisualTreeAsset.", this);
                return;
            }

            _panelSettings = panelSettingsAsset != null
                ? Instantiate(panelSettingsAsset)
                : ScriptableObject.CreateInstance<PanelSettings>();
            _panelSettings.sortingOrder = sortingOrder;

            GameObject documentObject = new GameObject("LevelUpCardsTestDocument");
            documentObject.transform.SetParent(transform, false);

            UIDocument document = documentObject.AddComponent<UIDocument>();
            document.panelSettings = _panelSettings;
            document.visualTreeAsset = visualTreeAsset;

            LevelUpCardsView view = documentObject.AddComponent<LevelUpCardsView>();
            if (previewCards.Count > 0)
                view.SetCards(previewCards);

            _player = documentObject.AddComponent<LevelUpCardsDropInPlayer>();
        }

        private void OnDestroy()
        {
            if (_panelSettings != null)
                Destroy(_panelSettings);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FillDefaultAssets();
        }

        private void FillDefaultAssets()
        {
            if (visualTreeAsset == null)
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Work/CHUH/UI/unity/LevelUpCards.uxml");

            if (panelSettingsAsset == null)
                panelSettingsAsset = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
        }
#endif
    }
}
