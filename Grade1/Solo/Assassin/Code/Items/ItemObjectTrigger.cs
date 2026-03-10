using System;
using UnityEngine;

namespace Code.Items
{
    public class ItemObjectTrigger : MonoBehaviour
    {
        private IPickable _itemObject;

        private void Awake()
        {
            _itemObject = GetComponentInParent<IPickable>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
                _itemObject.PickUp();
        }
    }
}