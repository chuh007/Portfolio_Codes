using System;
using _01Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _01Scripts.UI.Inven
{
    public class ItemSlotUI : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected TextMeshProUGUI itemNameText;
        [SerializeField] protected TextMeshProUGUI amountText;
        [SerializeField] protected TextMeshProUGUI descriptionText;

        
        public RectTransform RectTrm => transform as RectTransform;
        public InventoryItem inventoryItem;

        public event Action<int> OnPointerDownEvent;
        protected int _slotIndex;

        public virtual void Initialize(int slotIndex)
        {
            _slotIndex = slotIndex;
        }
        
        public virtual void UpdateSlot(InventoryItem newItem)
        {
            inventoryItem = newItem;
            
            if(inventoryItem == null) return;
            itemNameText.text = inventoryItem.data.itemName;
            descriptionText.text = inventoryItem.data.description;

            if(inventoryItem.stackSize >= 1) 
                amountText.text = inventoryItem.stackSize.ToString();
            else
                amountText.text = string.Empty;
        }
        
        public virtual void CleanUpSlot()
        {
            inventoryItem = null;
            itemNameText.text = string.Empty;
            amountText.text = string.Empty;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                OnPointerDownEvent?.Invoke(_slotIndex); //내가 눌렸다는 것을 알림.
        }
    }
}