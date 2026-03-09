using System;
using _01Scripts.Core.EventSystem;
using _01Scripts.Items;
using Code.Core.GameSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01Scripts.Interact
{
    public class ItemObject : MonoBehaviour, IInteractable, ISavable
    {
        [field:SerializeField] public string Name { get; set; }
        public bool IsInteracted { get; private set; }
        [SerializeField] private ItemDataSO itemData;
        [SerializeField] private GameEventChannelSO inventoryChannel;
        [SerializeField] private Collider col;
        [SerializeField] private MeshRenderer meshRenderer;
        
        public void Interact()
        {
            IsInteracted = true;
            inventoryChannel.RaiseEvent(InventoryEvents.AddItemEvent.Initializer(itemData));
            col.enabled = false;
            meshRenderer.enabled = false;
        }

        [field: SerializeField] public SaveIdSO SaveID { get; private set; }

        [Serializable]
        public struct ItemObjectSaveData
        {
            public bool isInteracted;
        }
        
        public string GetSaveData()
        {
            ItemObjectSaveData data = new ItemObjectSaveData
            {
                isInteracted = IsInteracted,
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreData(string loadedData)
        {
            ItemObjectSaveData loadSaveData = JsonUtility.FromJson<ItemObjectSaveData>(loadedData);
            if (loadSaveData.isInteracted)
            {
                IsInteracted = true;
                col.enabled = false;
                meshRenderer.enabled = false;
            }
        }
    }
}