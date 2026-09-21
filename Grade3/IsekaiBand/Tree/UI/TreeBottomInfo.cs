using System.Collections.Generic;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.Tree.UI
{
    public class TreeBottomInfo : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nodeNameText;
        [SerializeField] private TextMeshProUGUI nodeDescriptionText;
        [SerializeField] private Button nodeActiveBtn;
        [SerializeField] private CanvasGroup nodeActiveCanvasGroup;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image costIconImage;
        [SerializeField] private Sprite coinCostIcon;
        [SerializeField] private Sprite noteCostIcon;
        [SerializeField] private TextMeshProUGUI buttonBackText;
        [SerializeField, Range(0f, 1f)] private float disabledButtonAlpha = 0.6f;

        private readonly string ACTIVE_TEXT = "활성화";
        private readonly string CANTACTIVE_TEXT = "이전 노드 활성화 필요";
        
        private TreeNodeDataSO _nodeData;

        
        private void Awake()
        {
            Bus<NodeSelectEvent>.OnEvent += HandleTreeInfoChange;
            nodeActiveBtn.onClick.AddListener(HandleActive);
        }
        
        private void OnDestroy()
        {
            Bus<NodeSelectEvent>.OnEvent -= HandleTreeInfoChange;
        }
        
        private void HandleTreeInfoChange(NodeSelectEvent evt)
        {
            
            _nodeData = evt.NodeData;
            iconImage.sprite = evt.NodeData.icon;
            nodeNameText.SetText(evt.NodeData.nodeName);
            costText.SetText(evt.NodeData.cost.ToString());
            if (costIconImage != null)
            {
                costIconImage.sprite = evt.NodeData.costCurrency == TreeNodeCostCurrency.Note
                    ? noteCostIcon
                    : coinCostIcon;
            }
            nodeDescriptionText.SetText(evt.NodeData.description);
            UpdateActivationButtonState();
        }

        private void UpdateActivationButtonState()
        {
            bool isActivated = TreeDataManager.Instance.IsNodeAvailable(_nodeData.nodeID);
            nodeActiveBtn.gameObject.SetActive(!isActivated);

            if (isActivated)
            {
                SetActivationButtonInteractable(false);
                buttonBackText.SetText(ACTIVE_TEXT);
                return;
            }

            bool hasRequiredNode = TreeDataManager.Instance.CanActivateNode(_nodeData.nodeID);
            bool hasEnoughCost = TreeDataManager.Instance.IsEnoughCost(_nodeData.nodeID);

            SetActivationButtonInteractable(hasRequiredNode && hasEnoughCost);
            buttonBackText.SetText(hasRequiredNode ? ACTIVE_TEXT : CANTACTIVE_TEXT);
        }

        private void SetActivationButtonInteractable(bool isInteractable)
        {
            nodeActiveBtn.interactable = isInteractable;
            nodeActiveCanvasGroup.alpha = isInteractable ? 1f : disabledButtonAlpha;
        }

        private void HandleActive()
        {
            bool ans = TreeDataManager.Instance.TryActivateNode(_nodeData.nodeID);
            if (ans)
            {
                UpdateActivationButtonState();
            }
        }
    }
}
