using System.Collections.Generic;
using _01Scripts.Items;

namespace _01Scripts.Core.EventSystem
{
    public static class InventoryEvents
    {
        public static readonly InventoryDataEvent InventoryDataEvent = new InventoryDataEvent();
        public static readonly RequestInventoryDataEvent RequestInventoryDataEvent = new RequestInventoryDataEvent();
        public static readonly AddItemEvent AddItemEvent = new AddItemEvent();
    }
    
    public class InventoryDataEvent : GameEvent
    {
        public int slotCount;
        public List<InventoryItem> items;

        public InventoryDataEvent Initializer(int slotCount, List<InventoryItem> items)
        {
            this.slotCount = slotCount;
            this.items = items;
            return this;
        }
    }
    
    public class RequestInventoryDataEvent : GameEvent { }
    
    public class AddItemEvent : GameEvent
    {
        public ItemDataSO itemData;

        public AddItemEvent Initializer(ItemDataSO itemData)
        {
            this.itemData = itemData;
            return this;
        }
    }
}