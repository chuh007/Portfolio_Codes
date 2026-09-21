using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepSlashes
    {
        private readonly Phase2LaneSweepPatternSO _pattern;

        public LaneSweepSlashes(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public async UniTask SweepDangerLanes(Enemy owner, LaneSweep sweep, DamageData damage, CancellationToken ct)
        {
            int slashCount = Mathf.Max(5, _pattern.SlashCountPerLane);
            for (int slash = 0; slash < slashCount; slash++)
            {
                if (LaneSweepSequence.ShouldStop(owner, ct))
                    return;

                float t = slash / (slashCount - 1f);
                for (int i = 0; i < sweep.LaneRects.Length; i++)
                {
                    Rect segmentRect = LaneSweepPlan.GetSlashSegmentRect(sweep.LaneRects[i], slash, slashCount);
                    SpawnSlash(owner, segmentRect, damage);
                }

                if (slash < slashCount - 1 && _pattern.SlashSpawnDelay > 0f)
                    await UniTask.WaitForSeconds(_pattern.SlashSpawnDelay, cancellationToken: ct).SuppressCancellationThrow();
            }
        }

        public void SpawnSlash(Enemy owner, Rect segmentRect, DamageData damage)
        {
            if (_pattern.poolManager == null || _pattern.LaneSweepVFXItem == null)
                return;

            var pooled = _pattern.poolManager.Pop(_pattern.LaneSweepVFXItem);
            if (pooled is not IPooledVFX vfx)
            {
                if (pooled != null)
                    _pattern.poolManager.Push(pooled);
                return;
            }

            Vector2 direction = _pattern.LaneSweepDirection.sqrMagnitude > 0.001f ? _pattern.LaneSweepDirection.normalized : Vector2.left;
            vfx.Play(segmentRect.center, _pattern.LaneSweepVFXScale, direction);
            FitSlashVisualToSegment(pooled.gameObject, segmentRect);
            pooled.gameObject.GetComponent<VFXDamageBox>()
                ?.HitRepeatedAndGetCount(damage, direction, owner, segmentRect.size, _pattern.DamageTickInterval);
        }

        public void FitSlashVisualToSegment(GameObject slash, Rect segmentRect)
        {
            Renderer renderer = slash.GetComponentInChildren<Renderer>();
            if (renderer == null)
                return;

            float targetHeight = segmentRect.height * Mathf.Max(0.01f, _pattern.LaneSweepVFXScale);
            if (renderer.bounds.size.y > Mathf.Epsilon)
                slash.transform.localScale *= targetHeight / renderer.bounds.size.y;

            renderer = slash.GetComponentInChildren<Renderer>();
            Vector3 offset = (Vector3)segmentRect.center - renderer.bounds.center;
            slash.transform.position += offset;
        }

        public async UniTask WaitUntilNextSweep(float sweepStartTime, CancellationToken ct)
        {
            float waitTime = Mathf.Max(0f, _pattern.SweepInterval - (Time.time - sweepStartTime));
            await UniTask.WaitForSeconds(waitTime, cancellationToken: ct).SuppressCancellationThrow();
        }
    }
}
