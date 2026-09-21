using UnityEngine;
using _Code.LCH._02.Scripts.Player.Attack;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PunkSpectrumLines
    {
        public const int BarCount = 72;
        public LineRenderer[] Bars { get; } = new LineRenderer[BarCount];
        public LineRenderer InnerRing { get; private set; }
        private Material _material;

        public void Create(Transform parent)
        {
            if (InnerRing != null) return;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _material = new Material(shader)
                {
                    name = "PunkBandSpectrumRuntimeMaterial",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            for (int i = 0; i < BarCount; i++)
                Bars[i] = CreateLineRenderer(parent, $"PunkSpectrumBar_{i:00}", 2, false, 45);

            InnerRing = CreateLineRenderer(
                parent, "PunkSpectrumInnerRing",
                BarCount,
                true,
                44);
        }

        private LineRenderer CreateLineRenderer(
            Transform parent, string objectName,
            int positionCount,
            bool loop,
            int sortingOrder)
        {
            var lineObject = new GameObject(objectName);
            lineObject.transform.SetParent(parent, false);
            var line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = loop;
            line.positionCount = positionCount;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            GroundEffectRenderLayer.ApplyTo(line, sortingOrder);
            if (_material != null)
                line.sharedMaterial = _material;
            return line;
        }

        public void Dispose()
        {
            if (_material != null) Object.Destroy(_material);
        }
    }
}
