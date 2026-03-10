using Code.Entities;
using UnityEngine;

namespace Code.Players
{
    public class PlayerTargetFinderCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform CheckTrm;
        [SerializeField] private float range;
        [SerializeField] private LayerMask layer;

        private Player _player;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            CheckTrm = _player.transform;
        }

        public Collider2D FindProximateTargetsInCicle()
        {
            CheckTrm = _player.transform;
            Collider2D[] targets = Physics2D.OverlapCircleAll(CheckTrm.position, range, layer);
            if (targets.Length > 0) return targets[0];
            return null;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.7f, 0.7f, 0, 1f);
            Gizmos.DrawWireSphere(CheckTrm.position, range);
        }
#endif
    }
}

