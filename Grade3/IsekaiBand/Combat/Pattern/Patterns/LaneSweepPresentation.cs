using System.Threading;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepPresentation
    {
        private readonly Phase2LaneSweepPatternSO _pattern;
        private const float HiddenDistanceFromTarget = 100f;
        private Vector3 _returnPosition;
        private bool _hasReturnPosition;
        private bool _usedVanishAnimation;
        public LaneSweepPresentation(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public async UniTask HideOwner(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            _usedVanishAnimation = await (_pattern.VanishPresentation ?? new PatternVanishPresentation()).VanishAsync(owner, ct);
            if (ct.IsCancellationRequested)
                return;

            _returnPosition = GetReturnPosition(owner);
            _hasReturnPosition = true;
            owner.SetAutomaticRepositionSuppressed(true);
            MoveOwnerFarAway(owner);
        }

        public async UniTask Cleanup(Enemy owner)
        {
            if (owner != null && _hasReturnPosition)
                owner.transform.position = _returnPosition;

            if (owner != null)
                await (_pattern.VanishPresentation ?? new PatternVanishPresentation()).ReturnAsync(
                    owner,
                    _usedVanishAnimation,
                    CancellationToken.None);

            if (owner != null)
                owner.SetAutomaticRepositionSuppressed(false);

            _hasReturnPosition = false;
            _usedVanishAnimation = false;

            await _pattern.Camera.RestoreCamera();
            _pattern.Plan.Sweeps.Clear();
        }

        public static void MoveOwnerFarAway(Enemy owner)
        {
            Vector2 ownerPosition = owner.transform.position;
            Vector2 targetPosition = owner.target != null ? owner.target.transform.position : ownerPosition;
            Vector2 direction = ownerPosition - targetPosition;
            if (direction.sqrMagnitude < 0.001f)
                direction = Vector2.right;

            Vector2 hiddenPosition = targetPosition + direction.normalized * HiddenDistanceFromTarget;
            owner.transform.position = new Vector3(hiddenPosition.x, hiddenPosition.y, owner.transform.position.z);
        }

        public static Vector3 GetReturnPosition(Enemy owner)
        {
            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper == null || !stageHelper.IsBossArenaActive)
                return owner.transform.position;

            Vector2 center = stageHelper.BossArenaCenter;
            return new Vector3(center.x, center.y, owner.transform.position.z);
        }
    }
}
