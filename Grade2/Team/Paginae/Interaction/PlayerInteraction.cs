using Blade.Entities;
using UnityEngine;

namespace Work.CUH.Code.Interaction
{
    public class PlayerInteraction : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatisTarget;
        [SerializeField] private float interactionRange = 2f;
        
        private Entity _entity;
        private RaycastHit[] _results = new RaycastHit[5];
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void CheckInteraction()
        {
            var size = Physics.RaycastNonAlloc(transform.position, transform.forward * interactionRange, _results, 10f, whatisTarget);
            for (var i = 0; i < size; i++)
            {
                if (_results[i].collider && _results[i].collider.TryGetComponent(out IInteraction interaction))
                {
                    interaction.Interact();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * interactionRange);
        }
    }
}