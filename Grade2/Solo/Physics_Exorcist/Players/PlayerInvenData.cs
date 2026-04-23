using System;
using System.Collections.Generic;
using System.Linq;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using _01Scripts.Items;
using _01Scripts.Items.Inven;
using Code.Core.GameSystem;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerInvenData : InvenData, IEntityComponent, IAfterInitialize, ISavable
    {
        public GameEventChannelSO inventoryChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        
        #region Temp

        public ItemDataSO healItemData;
        public ItemDataSO attackItemData;

        #endregion
        
        private Player _player;
        private EntityStat _statCompo;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            inventory = new List<InventoryItem>(); //인벤토리에 빈칸 만들어주고
            
            _statCompo = entity.GetCompo<EntityStat>();
        }

        public void AfterInitialize()
        {
            inventoryChannel.AddListener<RequestInventoryDataEvent>(HandleRequestPlayerInvenData);
            inventoryChannel.AddListener<AddItemEvent>(HandleAddItem);
        }
        
        private void OnDestroy()
        {
            inventoryChannel.RemoveListener<RequestInventoryDataEvent>(HandleRequestPlayerInvenData);
            inventoryChannel.RemoveListener<AddItemEvent>(HandleAddItem);
        }
        
        private void HandleRequestPlayerInvenData(RequestInventoryDataEvent evt)
        {
            UpdateInventoryUI();
        }

        private void UpdateInventoryUI()
        {

            inventoryChannel.RaiseEvent(
                InventoryEvents.InventoryDataEvent.Initializer(2, inventory));
        }
        private void HandleAddItem(AddItemEvent evt) => AddItem(evt.itemData);
        
        public override void AddItem(ItemDataSO itemData, int count = 1)
        {
            IEnumerable<InventoryItem> items = GetItems(itemData);
            InventoryItem canAddItem = items.FirstOrDefault(item => item.IsFullStack == false);

            if (canAddItem == default)
            {
                CreateNewInventory(itemData, count);
            }
            else
            {
                int remain = canAddItem.AddStack(count); 
                if(remain > 0)
                    CreateNewInventory(itemData, remain);
            }
        }
        
        private void CreateNewInventory(ItemDataSO itemData, int count)
        {
            InventoryItem newItem = new InventoryItem(itemData, count);
            inventory.Add(newItem);
        }

        public override void RemoveItem(ItemDataSO itemData, int count = 1)
        {
            IEnumerable<InventoryItem> items = GetItems(itemData);
            InventoryItem canAddItem = items.FirstOrDefault();
            if (canAddItem == default)
            {
                Debug.Log("버그다버그");
            }
            else
            {
                if(canAddItem.stackSize == 0) return;
                canAddItem.RemoveStack(count);

                UpdateInventoryUI();
            }
            UpdateInventoryUI();
        }
        
        public override bool CanAddItem(ItemDataSO itemData)
        {
            if (inventory.Count < 99 - 1) return true;
            
            IEnumerable<InventoryItem> items = GetItems(itemData);
            InventoryItem canAddItem = items.FirstOrDefault(item => item.IsFullStack == false);
            
            return canAddItem != default; 
        }

        public override bool CanRemoveItem(ItemDataSO itemData)
        {
            IEnumerable<InventoryItem> items = GetItems(itemData);
            InventoryItem canAddItem = items.FirstOrDefault();
            return canAddItem != null && canAddItem.stackSize != 0;
        }

#if UNITY_EDITOR
        [ContextMenu("Load all item data")]
        private void LoadAllItemData()
        {
            if (itemDB == null)
            {
                Debug.LogWarning("No item DB found");
                return;
            }

            string path = "Assets/08SO/ItemData";
            string[] assetNames = AssetDatabase.FindAssets("", new[] {path});
            itemDB.ClearAllItems();
            foreach (string guid in assetNames)
            {
                string soPath = AssetDatabase.GUIDToAssetPath(guid);
                ItemDataSO itemData = AssetDatabase.LoadAssetAtPath<ItemDataSO>(soPath);
                if (itemData != null)
                {
                    if(itemData is ItemDataSO scrapItemDataSO)
                        itemDB.scrapItems.Add(scrapItemDataSO);
                }
            }
            
            EditorUtility.SetDirty(itemDB);
            AssetDatabase.SaveAssets();
        }
        

#endif
        
        #region SaveLogic

        [SerializeField] private ItemDatabaseSO itemDB;
        [field: SerializeField] public SaveIdSO SaveID { get; private set; }

        [Serializable]
        public struct InvenItemSaveData
        {
            public string itemId;
            public int stackSize;
            public int slotIndex;
        }

        
        [Serializable]
        public struct InvenSaveData
        {
            public List<InvenItemSaveData> items;
        }
        
        public string GetSaveData()
        {
            
            InvenSaveData saveData;
            saveData.items = inventory.Select((item, idx) => new InvenItemSaveData
            {
                itemId = item.data.itemID,
                stackSize = item.stackSize,
                slotIndex = idx
            }).ToList();

            
            return JsonUtility.ToJson(saveData);
        }
        
        public void RestoreData(string loadedData)
        {
            InvenSaveData loadedSaveData = JsonUtility.FromJson<InvenSaveData>(loadedData);
            foreach (InvenItemSaveData itemData in loadedSaveData.items)
            {
            }
            loadedSaveData.items.Sort((item1, item2) => item1.slotIndex - item2.slotIndex);
            inventory = loadedSaveData.items.Select(saveItem =>
            {
                ItemDataSO itemData = itemDB.GetItem(saveItem.itemId);
                Debug.Assert(itemData != null, $"Save data corrupted : {saveItem.itemId} is not exist on DB");
                
                return new InventoryItem(itemData, saveItem.stackSize);
            }).ToList();
        }

        #endregion
    }
}