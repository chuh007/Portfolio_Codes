using _01Scripts.Entities;
using Chuh007Lib.StatSystem;
using UnityEditor;
using UnityEngine;

namespace _01Scripts.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "SO/Items/Item", order = 0)]
    public class ItemDataSO : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public string itemID;
        public int maxStack;
        
        [TextArea]  public string description;
        public string GetDescription() => description;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            string path = AssetDatabase.GetAssetPath(this);
            itemID = AssetDatabase.AssetPathToGUID(path);
        }
#endif
    }
}