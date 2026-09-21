using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Tree
{
    [CreateAssetMenu(fileName = "TreeNodeDatabase", menuName = "SO/Tree/TreeNodeDatabase", order = 0)]
    public class TreeNodeDatabaseSO : ScriptableObject
    {
        public List<TreeNodeDataSO> treeNodes = new();
    }
}
