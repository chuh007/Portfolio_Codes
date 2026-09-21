using System;
using System.Collections.Generic;
using System.Linq;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Core.Persistence;
using _Work.CHUH.Code.Resource;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Tree
{
    public class TreeDataManager : MonoBehaviour
    {
        [SerializeField] private TreeNodeDatabaseSO database;
        
        public static TreeDataManager Instance { get; private set; }
        
        // 모든 특성 노드 데이터를 식별 ID로 저장하는 딕셔너리
        public readonly Dictionary<string, TreeNodeDataSO> AllNodes = new();
        
        // 활성화된 특성 노드 ID 목록
        private HashSet<string> _activatedNodes = new HashSet<string>();
        
        // 각 노드의 현재 진입차수 기록
        private Dictionary<string, int> _nodeInDegrees = new Dictionary<string, int>();
        
        public event Action OnNodeActivated;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var treeNode in database.treeNodes)
            {
                AllNodes.Add(treeNode.nodeID, treeNode);   
            }
            InitializeInDegrees();
            RestoreActivatedNodes();
        }

        private void Start()
        {
            ApplyActivatedNodeUpgrades();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
                PersistentProgressStore.Flush();
        }

        private void OnApplicationQuit()
        {
            PersistentProgressStore.Flush();
        }
        
        private void InitializeInDegrees()
        {
            _nodeInDegrees.Clear();

            foreach (var (nodeName, _) in AllNodes)
            {
                _nodeInDegrees[nodeName] = 0; // 0 기본으로 깔고
            }
            foreach (var (_, nodeData) in AllNodes)
            {
                foreach (var child in nodeData.childNodes)
                {
                    _nodeInDegrees[child.nodeID]++; // 진입차수 맞추기
                }
            }
        }
        
        public bool TryActivateNode(string nodeId)
        {
            if (!AllNodes.TryGetValue(nodeId, out TreeNodeDataSO nodeData)) 
            {
                Debug.LogError($"ID 없음 : {nodeId}");
                return false;
            }
            if (IsNodeAvailable(nodeId) || !CanActivateNode(nodeId) || !IsEnoughCost(nodeId)) return false;

            if (!TrySpendCost(nodeData))
                return false;

            _activatedNodes.Add(nodeId);
            UpdateChildrenInDegrees(nodeData);
            SaveActivatedNodes();

            foreach (var upgrade in nodeData.upgrades)
            {
                if (upgrade == null) continue;
                upgrade.Apply();
            }
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.SkillTreeActivated, SoundType.SFX));
            OnNodeActivated?.Invoke();
            return true;
        }

        private void RestoreActivatedNodes()
        {
            foreach (string nodeId in PersistentProgressStore.GetActivatedTreeNodeIds())
            {
                if (!AllNodes.ContainsKey(nodeId)) continue;
                _activatedNodes.Add(nodeId);
            }

            foreach (string nodeId in _activatedNodes)
                UpdateChildrenInDegrees(AllNodes[nodeId]);
        }

        private void ApplyActivatedNodeUpgrades()
        {
            foreach (TreeNodeDataSO nodeData in database.treeNodes)
            {
                if (nodeData == null || !_activatedNodes.Contains(nodeData.nodeID)) continue;

                foreach (var upgrade in nodeData.upgrades)
                {
                    if (upgrade != null)
                        upgrade.Apply();
                }
            }
        }

        private void SaveActivatedNodes()
        {
            PersistentProgressStore.SetActivatedTreeNodeIds(_activatedNodes);
            PersistentProgressStore.Flush();
        }
        
        private void UpdateChildrenInDegrees(TreeNodeDataSO activatedNode)
        {
            foreach (var childID in activatedNode.childNodes)
            {
                if (_nodeInDegrees.ContainsKey(childID.nodeID))
                {
                    _nodeInDegrees[childID.nodeID]--;
                }
            }
        }
        
        /// <summary>
        /// 노드가 이미 찍힌 노드인지 확인하는 메서드
        /// </summary>
        /// <param name="nodeID">Node Data의 string 쓰기. 손으로 적지 마세요</param>
        public bool IsNodeAvailable(string nodeID)
        {
            return _activatedNodes.Contains(nodeID);
        }
        
        /// <summary>
        /// 노드를 찍을 수 있는지 확인하는 메서드
        /// </summary>
        /// <param name="nodeID">Node Data의 string 쓰기. 손으로 적지 마세요</param>
        /// <returns></returns>
        public bool CanActivateNode(string nodeID)
        {
            return _nodeInDegrees[nodeID] == 0;
        }

        public bool IsEnoughCost(string nodeId)
        {
            if (SupplyManager.Instance == null || !AllNodes.TryGetValue(nodeId, out TreeNodeDataSO nodeData))
                return false;

            return nodeData.costCurrency switch
            {
                TreeNodeCostCurrency.Coin => SupplyManager.Instance.Coin >= nodeData.cost,
                TreeNodeCostCurrency.Note => SupplyManager.Instance.Note >= nodeData.cost,
                _ => false
            };
        }

        private static bool TrySpendCost(TreeNodeDataSO nodeData)
        {
            if (SupplyManager.Instance == null)
                return false;

            return nodeData.costCurrency switch
            {
                TreeNodeCostCurrency.Coin => SupplyManager.Instance.TryUseCoin(nodeData.cost),
                TreeNodeCostCurrency.Note => SupplyManager.Instance.TryUseNote(nodeData.cost),
                _ => false
            };
        }
        
#if UNITY_EDITOR
        [ContextMenu("Check Cycle")]
        public void HasCycle()
        {
            Dictionary<string, int> inDegreeMap = new Dictionary<string, int>();

            foreach (var nodeId in AllNodes.Keys)
            {
                inDegreeMap[nodeId] = 0;
            }
            foreach (var node in AllNodes.Values)
            {
                foreach (var child in node.childNodes)
                {
                    if (inDegreeMap.ContainsKey(child.nodeID))
                    {
                        inDegreeMap[child.nodeID]++;
                    }
                }
            }
            Queue<string> queue = new Queue<string>();
            foreach (var kvp in inDegreeMap)
            {
                if (kvp.Value == 0)
                {
                    queue.Enqueue(kvp.Key);
                }
            }
            
            int visitedCount = 0;
            while (queue.Count > 0)
            {
                string currentId = queue.Dequeue();
                visitedCount++;

                if (AllNodes.TryGetValue(currentId, out var nodeData))
                {
                    foreach (var child in nodeData.childNodes)
                    {
                        inDegreeMap[child.nodeID]--;
                        
                        if (inDegreeMap[child.nodeID] == 0) queue.Enqueue(child.nodeID);
                    }
                }
            }
            
            bool hasCycle = visitedCount != AllNodes.Count;
            
            if (hasCycle)
                Debug.LogError($"사이클 감지 : {visitedCount} / {AllNodes.Count}");
        }
#endif
    }
}
