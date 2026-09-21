using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoFloorDecoration
    {
        private readonly PianoBossRuntime _runtime;
        private readonly List<GameObject> _blackKeys = new();
        public void Clear() => _blackKeys.Clear();

        public PianoFloorDecoration(PianoBossRuntime runtime) => _runtime = runtime;

        public void BuildWhiteKeyOutlines(Rect bounds)
        {
            Color outlineColor = new Color(0.1f, 0.1f, 0.13f, 0.32f);
            float whiteWidth = bounds.width / _runtime.KeyCount;
            float thickness = Mathf.Clamp(Mathf.Min(whiteWidth, bounds.height) * 0.018f, 0.015f, 0.04f);

            for (int i = 0; i <= _runtime.KeyCount; i++)
            {
                float x = bounds.xMin + whiteWidth * i;
                CreateOutlineBar(
                    $"WhiteKeyOutline_V_{i}",
                    new Rect(x - thickness * 0.5f, bounds.yMin, thickness, bounds.height),
                    outlineColor);
            }

            CreateOutlineBar(
                "WhiteKeyOutline_Top",
                new Rect(bounds.xMin, bounds.yMax - thickness * 0.5f, bounds.width, thickness),
                outlineColor);
            CreateOutlineBar(
                "WhiteKeyOutline_Bottom",
                new Rect(bounds.xMin, bounds.yMin - thickness * 0.5f, bounds.width, thickness),
                outlineColor);
        }

        public void BuildBlackKeys(Rect bounds)
        {
            float whiteWidth = bounds.width / _runtime.KeyCount;
            float blackWidth = whiteWidth * 0.55f;
            float blackHeight = bounds.height * 0.48f;
            Color opaqueBlackKeyColor = PianoFloorVisuals.OpaqueIfTransparent(_runtime.BlackKeyColor);

            for (int i = 0; i < _runtime.KeyCount - 1; i++)
            {
                int note = i % 7;
                if (note != 0 && note != 1 && note != 3 && note != 4 && note != 5)
                    continue;

                Rect whiteRect = _runtime.GetKeyRect(i);
                Vector2 center = new Vector2(whiteRect.xMax, bounds.yMax - blackHeight * 0.5f);
                var blackRect = new Rect(center - new Vector2(blackWidth, blackHeight) * 0.5f, new Vector2(blackWidth, blackHeight));
                GameObject key = _runtime.SpriteFactory.CreateSpriteObject($"BlackKey_{i}", _runtime.SpriteFactory.GetSolidSprite(), blackRect.center, opaqueBlackKeyColor, _runtime.FloorSortingOrder + 3);
                if (key == null)
                    continue;

                key.transform.SetParent(_runtime.FloorVisuals.Root.transform, true);
                _runtime.SpriteFactory.FitSpriteToRect(key, blackRect);
                _blackKeys.Add(key);
            }
        }

        public void CreateOutlineBar(string objectName, Rect rect, Color color)
        {
            GameObject bar = _runtime.SpriteFactory.CreateSpriteObject(objectName, _runtime.SpriteFactory.GetSolidSprite(), rect.center, color, _runtime.FloorSortingOrder + 1);
            if (bar == null)
                return;

            bar.transform.SetParent(_runtime.FloorVisuals.Root.transform, true);
            _runtime.SpriteFactory.FitSpriteToRect(bar, rect);
        }
    }
}
