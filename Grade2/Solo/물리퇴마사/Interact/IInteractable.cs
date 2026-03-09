using UnityEngine;

namespace _01Scripts.Interact
{
    public interface IInteractable
    {
        string Name { get; }
        bool IsInteracted { get; }

        public void Interact();
    }
}