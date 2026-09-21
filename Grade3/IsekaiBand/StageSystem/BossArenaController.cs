using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    public class BossArenaController
    {
        private readonly BossArenaBoundaryBuilder _boundaries;
        private readonly BossArenaCameraBoundsController _cameraBoundsController;
        private readonly BossArenaGroundCollisionOverride _groundCollisionOverride;
        private readonly float _wallThickness;
        private Enemy _boss;
        private GameObject _parent => _boundaries.Parent;

        public Enemy Boss => _boss;
        public bool IsActive => _parent != null && _parent.activeSelf;

        public BossArenaController(
            Transform owner,
            SpriteRenderer background,
            int wallLayer,
            Vector2 arenaSize,
            Sprite cornerSprite,
            Material chainMaterial,
            float wallThickness,
            Color wallColor,
            float cameraOrthographicSize,
            float cameraSizeTransitionDuration)
        {
            _boundaries = new BossArenaBoundaryBuilder(
                owner, background, wallLayer, arenaSize, cornerSprite, chainMaterial, wallThickness, wallColor);
            _wallThickness = wallThickness;
            _cameraBoundsController = new BossArenaCameraBoundsController(
                arenaSize, cameraOrthographicSize, cameraSizeTransitionDuration);
            _groundCollisionOverride = new BossArenaGroundCollisionOverride();
        }

        public void Prepare() => _boundaries.Prepare();

        public void Create(Enemy boss, Vector2 center)
        {
            Deactivate();
            Prepare();

            _boss = boss;
            _parent.transform.position = center;

            Vector2 arenaSize = _boundaries.Size;
            StageHelper.Instance?.SetBossArenaBounds(center, arenaSize);
            KeepBossInsideArena(boss);
            _groundCollisionOverride.ApplyTo(boss);
            _cameraBoundsController.Activate(center);
            _parent.SetActive(true);
        }

        public void ReplaceBoss(Enemy boss)
        {
            _boss = boss;
            if (IsActive)
            {
                KeepBossInsideArena(boss);
                _groundCollisionOverride.ApplyTo(boss);
            }
        }

        public void SetCameraOrthographicSize(float orthographicSize)
            => _cameraBoundsController.SetTargetOrthographicSize(orthographicSize);

        public void Deactivate()
        {
            _groundCollisionOverride.Restore();

            if (_parent != null)
                _parent.SetActive(false);

            _cameraBoundsController.Deactivate();
            StageHelper.Instance?.ClearBossArenaBounds();
            _boss = null;
        }

        public void Dispose()
        {
            Deactivate();
            _boundaries.Dispose();
        }

        private void KeepBossInsideArena(Enemy boss)
        {
            StageHelper stageHelper = StageHelper.Instance;
            if (boss == null || stageHelper == null || !stageHelper.IsBossArenaActive)
                return;

            Vector2 clampedPosition = stageHelper.ClampToMapBound(boss.transform.position, _wallThickness);
            boss.transform.position = new Vector3(clampedPosition.x, clampedPosition.y, boss.transform.position.z);
        }
    }
}
