using System.Collections.Generic;
using _Work.CHUH.Code.Tree.Upgrade;
using UnityEngine;

namespace _Work.CHUH.Code.Tree
{
    public enum TreeNodeCostCurrency
    {
        Coin,
        Note
    }

    [CreateAssetMenu(fileName = "TreeNodeData", menuName = "SO/Tree/TreeNodeData", order = 0)]
    public class TreeNodeDataSO : ScriptableObject
    {
        [HideInInspector] public Vector2 graphPosition; 

        [Tooltip("런타임 트리 UI에서 사용할 세로 행. -1이면 연결 관계로 자동 배치합니다.")]
        public int uiRow = -1;
        [Tooltip("같은 UI 행 안에서 왼쪽부터 배치될 순서입니다.")]
        public int uiOrder;
        
        /// <summary>
        /// 노드 식별 고유 ID, 에디터가 자동으로 무작위 값 집어넣어줌.
        /// </summary>
        public string nodeID;
        /// <summary>
        /// 실제 UI상 이름
        /// </summary>
        public string nodeName;
        /// <summary>
        /// 실제 UI상 설명
        /// </summary>
        [TextArea] public string description;
        [Tooltip("노드를 활성화할 때 소비할 재화입니다.")]
        public TreeNodeCostCurrency costCurrency = TreeNodeCostCurrency.Coin;
        public int cost;
        public Sprite icon;
        /// <summary>
        /// 자식 노드들
        /// </summary>
        public List<TreeNodeDataSO> childNodes = new();
        /// <summary>
        /// 이 노드에 달린 업그레이드들
        /// </summary>
        public List<BaseUpgradeSO> upgrades = new();
    }
}
