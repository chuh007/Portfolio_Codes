using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DnaHelixSequence
    {
        private readonly PianoBossDnaHelixPatternSO _pattern;

        public DnaHelixSequence(PianoBossDnaHelixPatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            PianoBossRuntime runtime = _pattern.RuntimeFor(owner);
            using (runtime.BeginPerformance())
            {
                _pattern.EnsureFloor(owner, runtime, _pattern.GetDamage(owner, _pattern.FloorMultiplier));

                Rect arenaBounds = _pattern.ArenaBounds(owner);
                ResolveVerticalTravelBounds(arenaBounds, out float startY, out float endY);
                DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
                float initialPhase = Random.Range(0f, Mathf.PI * 2f);
                int strandPairCount = Mathf.Max(1, _pattern.PairCount);
                int columnCount = Mathf.Max(1, _pattern.HelixCount);
                float columnWidth = arenaBounds.width / columnCount;
                float amplitude = Mathf.Min(
                    _pattern.HorizontalAmplitude,
                    Mathf.Max(0f, columnWidth * 0.5f - _pattern.HelixEdgePadding));

                for (int pairIndex = 0; pairIndex < strandPairCount; pairIndex++)
                {
                    for (int helixIndex = 0; helixIndex < columnCount; helixIndex++)
                    {
                        float centerX = arenaBounds.xMin + columnWidth * (helixIndex + 0.5f);
                        float phaseOffset = Mathf.PI * 2f * helixIndex / columnCount;
                        _pattern.Spawn.SpawnHelixPair(
                            owner,
                            centerX,
                            startY,
                            endY,
                            amplitude,
                            initialPhase + phaseOffset,
                            damage,
                            pairIndex + helixIndex);
                    }

                    if (pairIndex + 1 < strandPairCount)
                        await UniTask.WaitForSeconds(_pattern.SpawnInterval, cancellationToken: ct);
                }

                await UniTask.WaitForSeconds(
                    _pattern.TravelDuration * 0.65f,
                    cancellationToken: ct);
            }
        }

        public void ResolveVerticalTravelBounds(
            Rect arenaBounds,
            out float startY,
            out float endY)
        {
            startY = arenaBounds.yMax + _pattern.CameraEdgePadding;
            endY = arenaBounds.yMin - _pattern.CameraEdgePadding;

            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.orthographic)
                return;

            float cameraCenterY = mainCamera.transform.position.y;
            float cameraHalfHeight = mainCamera.orthographicSize;
            startY = Mathf.Max(startY, cameraCenterY + cameraHalfHeight + _pattern.CameraEdgePadding);
            endY = Mathf.Min(endY, cameraCenterY - cameraHalfHeight - _pattern.CameraEdgePadding);
        }
    }
}
