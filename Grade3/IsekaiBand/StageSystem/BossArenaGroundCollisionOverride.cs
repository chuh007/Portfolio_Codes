using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaGroundCollisionOverride
    {
        private readonly struct ColliderState
        {
            public ColliderState(Collider2D collider, int excludeLayers)
            {
                Collider = collider;
                ExcludeLayers = excludeLayers;
            }

            public Collider2D Collider { get; }
            public int ExcludeLayers { get; }
        }

        private readonly int _groundLayerMask;
        private readonly List<ColliderState> _colliderStates = new();

        public BossArenaGroundCollisionOverride()
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            _groundLayerMask = groundLayer >= 0 ? 1 << groundLayer : 0;
        }

        public void ApplyTo(Enemy boss)
        {
            Restore();
            if (boss == null || _groundLayerMask == 0)
                return;

            Collider2D[] bossColliders = boss.GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D bossCollider in bossColliders)
            {
                if (bossCollider == null || bossCollider.isTrigger)
                    continue;

                int originalExcludeLayers = bossCollider.excludeLayers;
                _colliderStates.Add(new ColliderState(bossCollider, originalExcludeLayers));
                bossCollider.excludeLayers = originalExcludeLayers | _groundLayerMask;
            }
        }

        public void Restore()
        {
            foreach (ColliderState state in _colliderStates)
            {
                if (state.Collider != null)
                {
                    int currentExcludeLayers = state.Collider.excludeLayers;
                    state.Collider.excludeLayers =
                        (currentExcludeLayers & ~_groundLayerMask)
                        | (state.ExcludeLayers & _groundLayerMask);
                }
            }

            _colliderStates.Clear();
        }
    }
}
