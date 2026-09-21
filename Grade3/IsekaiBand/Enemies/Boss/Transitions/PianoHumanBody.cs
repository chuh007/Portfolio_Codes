using Chuh007Lib.Entities.Entities;
using _Work.CHUH.Code.EntityPlus;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoHumanBody
    {
        private Collider2D[] _colliders;
        public Enemy Owner { get; private set; }
        public EntityHealth Health { get; private set; }
        public EntityMover Mover { get; private set; }
        public PianoHumanAppearance Appearance { get; } = new();

        public void Initialize(Entity entity)
        {
            Owner = entity as Enemy;
            Health = entity.GetComponentInChildren<EntityHealth>(true);
            Mover = entity.GetComponentInChildren<EntityMover>(true);
            Appearance.Initialize(entity.GetComponentInChildren<EntityRenderer>(true));
            _colliders = entity.GetComponentsInChildren<Collider2D>(true);
        }

        public void Prepare()
        {
            Owner.enabled = false;
            if (Mover != null)
            {
                Mover.StopImmediately();
                Mover.CanManualMove = false;
            }
            SetCollidersEnabled(false);
        }

        public void Resume()
        {
            if (Owner != null)
                Owner.enabled = true;
            SetCollidersEnabled(true);
            if (Mover != null)
                Mover.CanManualMove = true;
        }

        public void Restore()
        {
            Resume();
            Appearance.Restore();
        }

        public void SetCollidersEnabled(bool enabled) => SetCollidersEnabled(_colliders, enabled);

        public static void SetCollidersEnabled(Collider2D[] colliders, bool enabled)
        {
            if (colliders == null)
                return;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    colliders[i].enabled = enabled;
            }
        }
    }
}
