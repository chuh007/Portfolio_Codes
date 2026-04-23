using System;
using System.Collections.Generic;
using _01Scripts.Items;
using UnityEngine;

namespace Code.Core.GameSystem
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "SO/Items/Database", order = 0)]
    public class ItemDatabaseSO : ScriptableObject
    {
        public List<ItemDataSO> scrapItems;
        
        private Dictionary<string, ItemDataSO> _itemDatabase = new Dictionary<string, ItemDataSO>();
        
        public ItemDataSO GetItem(string itemId)
            => _itemDatabase.GetValueOrDefault(itemId);
        
        public void ClearAllItems()
        {
            scrapItems?.Clear();
        }
        private void OnEnable()
        {
            if (scrapItems != null)
            {
                foreach (var scrapItem in scrapItems)
                {
                    _itemDatabase.Add(scrapItem.itemID, scrapItem);
                }
            }
        }
    }
}