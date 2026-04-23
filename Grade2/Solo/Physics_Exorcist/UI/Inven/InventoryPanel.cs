using System;
using System.Collections.Generic;
using _01Scripts.Core.EventSystem;
using _01Scripts.Items;
using UnityEngine;
using UnityEngine.Events;

namespace _01Scripts.UI.Inven
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO inventoryChannel;
        
        
        public RectTransform RectTrm => transform as RectTransform;
        
        public List<InventoryItem> inventory;
        
        [SerializeField] private Transform slotParentTrm;
        private ItemSlotUI[] _itemSlots;

        [HideInInspector] public int currentSlotCount;
        [HideInInspector] public ItemSlotUI selectedItem;
        [HideInInspector] public int selectedItemIndex;

        public UnityEvent<ItemDataSO> OnItemSelected;

        private void Awake()
        {
            _itemSlots = slotParentTrm.GetComponentsInChildren<ItemSlotUI>();
            for (int i = 0; i < _itemSlots.Length; i++)
            {
                _itemSlots[i].Initialize(i);
            }
            inventoryChannel.AddListener<InventoryDataEvent>(HandleDataRefresh);
        }

        public void OnDestroy()
        {
            inventoryChannel.RemoveListener<InventoryDataEvent>(HandleDataRefresh);
        }
        
        private void HandleDataRefresh(InventoryDataEvent evt)
        {
            inventory = evt.items; //받아온 아이템 갱신후
            currentSlotCount = evt.slotCount;
            UpdateSlotUI();
        }
        
        private void UpdateSlotUI()
        {
            for (int i = 0; i < currentSlotCount; i++)
            {
                _itemSlots[i].gameObject.SetActive(true);
                _itemSlots[i].CleanUpSlot();
            }
            for(int i = currentSlotCount; i < _itemSlots.Length; i++)
                _itemSlots[i].gameObject.SetActive(false);
            for (int i = 0; i < inventory.Count; i++)
                _itemSlots[i].UpdateSlot(inventory[i]);
            
        }
    }
}