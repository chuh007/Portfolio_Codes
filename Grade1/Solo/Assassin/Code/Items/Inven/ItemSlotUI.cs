using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Items.Inven
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] protected Image itemImage;
        [SerializeField] protected TextMeshProUGUI amountText;
        
        public InventoryItem inventoryItem;

        public void UpdateSlot(InventoryItem newItem)
        {
            inventoryItem = newItem;
            itemImage.color = Color.white;
            
            if(inventoryItem == null) return;
            
            itemImage.sprite = inventoryItem.data.icon;
            
            if(inventoryItem.stackSize > 1) 
                amountText.text = inventoryItem.stackSize.ToString();
            else
                amountText.text = string.Empty;
        }

        public void CleanUpSlot()
        {
            inventoryItem = null;
            itemImage.color = Color.clear;
            itemImage.sprite = null;
            amountText.text = string.Empty;
        }
    }
}