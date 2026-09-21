using UnityEngine;
using _Code.LCH._02.Scripts.Player.Attack;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PunkSpectrumPainter
    {
        private const float SpikeAngularWidth = 8f;
        private const float InnerRadiusRatio = 0.46f;
        private const float PrimaryResponseSpeed = 3.8f;
        private const float SecondaryResponseSpeed = 5.2f;
        private const float NoiseResponseSpeed = 2.1f;

        private static readonly Vector2[] Directions = CreateDirections();
        private float[,] _spikePeaks = new float[PunkSpectrumLines.BarCount, 0];
        private float _drawnRadius = float.NaN;
        private LineRenderer _drawnRing;

        private static Vector2[] CreateDirections()
        {
            var directions = new Vector2[PunkSpectrumLines.BarCount];
            for (int i = 0; i < directions.Length; i++)
            {
                float radians = (360f * i / PunkSpectrumLines.BarCount) * Mathf.Deg2Rad;
                directions[i] = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            }
            return directions;
        }

        internal void SetSpikes(float[] angles)
        {
            if (_spikePeaks.GetLength(1) != angles.Length)
                _spikePeaks = new float[PunkSpectrumLines.BarCount, angles.Length];
            for (int i = 0; i < PunkSpectrumLines.BarCount; i++)
            {
                float angle = 360f * i / PunkSpectrumLines.BarCount;
                for (int j = 0; j < angles.Length; j++)
                {
                    float delta = Mathf.Abs(Mathf.DeltaAngle(angle, angles[j]));
                    _spikePeaks[i, j] = delta > SpikeAngularWidth
                        ? -1f : Mathf.SmoothStep(0f, 1f, 1f - delta / SpikeAngularWidth);
                }
            }
        }

        public void Draw(PunkSpectrumLines lines, float baseRadius, Color color,
            float[] spikeAngles, float spikeRadius, float spikeDuration, float spikeTimeRemaining)
        {
            if (lines.InnerRing == null) return;

            float time = Time.time;
            float spikeStrength = spikeDuration > 0f
                ? Mathf.Clamp01(spikeTimeRemaining / spikeDuration)
                : 0f;
            spikeStrength = Mathf.SmoothStep(0f, 1f, spikeStrength);

            float innerRadius = baseRadius * InnerRadiusRatio;
            float maxCalmAmplitude = baseRadius - innerRadius;
            float calmBarWidth = Mathf.Max(0.04f, baseRadius * 0.025f);
            float spikeBarWidth = Mathf.Max(0.09f, baseRadius * 0.05f);
            float ringWidth = Mathf.Max(0.03f, baseRadius * 0.018f);

            bool geometryChanged = !_drawnRadius.Equals(baseRadius) || _drawnRing != lines.InnerRing;
            _drawnRadius = baseRadius;
            _drawnRing = lines.InnerRing;
            if (geometryChanged)
            {
                lines.InnerRing.startWidth = ringWidth;
                lines.InnerRing.endWidth = ringWidth;
            }
            Color calmColor = color;
            calmColor.a = Mathf.Clamp01(Mathf.Max(0.72f, color.a));
            Color spikeColor = new(1f, 0.76f, 0.12f, 1f);
            Color ringColor = color;
            ringColor.a = Mathf.Clamp01(Mathf.Max(0.65f, color.a * 0.78f));
            lines.InnerRing.startColor = ringColor;
            lines.InnerRing.endColor = ringColor;

            for (int i = 0; i < PunkSpectrumLines.BarCount; i++)
            {
                Vector2 direction = Directions[i];
                float primaryResponse = (Mathf.Sin(
                    time * (PrimaryResponseSpeed + i % 5 * 0.2f)
                    + i * 1.61f) + 1f) * 0.5f;
                float secondaryResponse = (Mathf.Sin(
                    time * (SecondaryResponseSpeed + i % 7 * 0.16f)
                    - i * 0.87f) + 1f) * 0.5f;
                float smoothNoise = Mathf.PerlinNoise(
                    i * 0.24f + 0.31f,
                    time * NoiseResponseSpeed);
                float spectrumResponse = primaryResponse * 0.34f
                                         + secondaryResponse * 0.26f
                                         + smoothNoise * 0.4f;
                float calmAmplitude = baseRadius
                                      * (0.24f + spectrumResponse * 0.34f);
                calmAmplitude = Mathf.Min(maxCalmAmplitude, calmAmplitude);
                float outerRadius = innerRadius + calmAmplitude;
                float strongestSpike = 0f;

                for (int spikeIndex = 0; spikeIndex < spikeAngles.Length; spikeIndex++)
                {
                    float peak = _spikePeaks[i, spikeIndex];
                    if (peak < 0f) continue;
                    float spike = peak * spikeStrength;
                    strongestSpike = Mathf.Max(strongestSpike, spike);
                    outerRadius = Mathf.Max(
                        outerRadius,
                        Mathf.Lerp(baseRadius, spikeRadius, spike));
                }

                Vector2 innerPosition = direction * innerRadius;
                Vector2 outerPosition = direction * outerRadius;
                LineRenderer bar = lines.Bars[i];
                float width = Mathf.Lerp(calmBarWidth, spikeBarWidth, strongestSpike);
                bar.startWidth = width;
                bar.endWidth = width;
                if (geometryChanged)
                {
                    bar.SetPosition(0, innerPosition);
                    lines.InnerRing.SetPosition(i, innerPosition);
                }
                bar.SetPosition(1, outerPosition);

                bar.startColor = Color.Lerp(calmColor, spikeColor, strongestSpike * 0.72f);
                bar.endColor = Color.Lerp(calmColor, spikeColor, strongestSpike);
            }
        }
    }
}
