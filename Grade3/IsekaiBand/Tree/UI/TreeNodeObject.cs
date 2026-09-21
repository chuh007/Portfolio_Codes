using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.Tree.UI
{
    /// <summary>
    /// TreeNodeDataSO를 담고, 보여준다.
    /// 선택하면 : 하얀 테두리로 주목시킴.
    /// 해금했으면 : 아이콘에 색 부여
    /// 해금할 수 있으면 : 아이콘이 흰 색임
    /// 해금 못하면 : 테두리, 아이콘 회색에 잠금 표시
    /// </summary>
    public class TreeNodeObject : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image outLineImage;
        [SerializeField] private Image lockImage;
        [SerializeField] private Color lockColor;
        [SerializeField] private Color activatedColor;
        
        private Button _button;

        public Transform lineTrm;
        
        public TreeNodeDataSO NodeData { get; private set; }
        
        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            Bus<UpdateNodeEvent>.OnEvent += UpdateUI;
            _button.onClick.AddListener(HandleNodeClick);
            iconImage.color = lockColor;
            outLineImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<UpdateNodeEvent>.OnEvent -= UpdateUI;
        }

        private void HandleNodeClick()
        {
            Bus<NodeSelectEvent>.Raise(new NodeSelectEvent(NodeData));
        }
        
        public void SetupData(TreeNodeDataSO nodeData)
        {
            NodeData = nodeData;
            iconImage.sprite = nodeData.icon;
        }
        
        public void UpdateUI(UpdateNodeEvent evt)
        {
            if (TreeDataManager.Instance.IsNodeAvailable(NodeData.nodeID))
            {
                Activate();
            }
            else if (TreeDataManager.Instance.CanActivateNode(NodeData.nodeID))
            {
                UnLock();
            }
        }
        
        // 활성화되지 않은 노드만 선택 테두리를 표시한다.
        public void Select()
        {
            bool isActivated = TreeDataManager.Instance.IsNodeAvailable(NodeData.nodeID);
            outLineImage.gameObject.SetActive(!isActivated);
        }
        
        public void UnSelect()
        {
            outLineImage.gameObject.SetActive(false);
        }
        
        // 열 수 있어지면 비주얼 업데이트
        public void UnLock()
        {
            iconImage.color = Color.white;
            lockImage.gameObject.SetActive(false);
        }
        
        // 열기를 선택하면 아예 열린게 됨.
        public void Activate()
        {
            iconImage.color = activatedColor;
            lockImage.gameObject.SetActive(false);
            outLineImage.gameObject.SetActive(false);
        }
        
    }
}
