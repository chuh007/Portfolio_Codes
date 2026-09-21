using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoFloorVisuals
    {
        private readonly PianoBossRuntime _runtime;
        private readonly List<KeyVisual> _keys = new();
        private GameObject _floorRoot;
        public GameObject Root => _floorRoot;
        public PianoFloorVisuals(PianoBossRuntime runtime) => _runtime = runtime;

        public void BuildPianoFloor(Rect bounds)
        {
            _floorRoot = new GameObject("PianoBossFloor");
            _floorRoot.transform.position = Vector3.zero;
            Color opaqueWhiteKeyColor = OpaqueIfTransparent(_runtime.WhiteKeyColor);

            for (int i = 0; i < _runtime.KeyCount; i++)
            {
                Rect rect = _runtime.GetKeyRect(i);
                GameObject key = _runtime.SpriteFactory.CreateSpriteObject($"WhiteKey_{i}", _runtime.Floor.KeySprite, rect.center, opaqueWhiteKeyColor, _runtime.FloorSortingOrder);
                if (key == null)
                    continue;

                key.transform.SetParent(_floorRoot.transform, true);
                _runtime.SpriteFactory.FitSpriteToRect(key, rect);

                SpriteRenderer renderer = key.GetComponent<SpriteRenderer>();
                _keys.Add(new KeyVisual(key, renderer, opaqueWhiteKeyColor));
            }

            _runtime.FloorDecoration.BuildWhiteKeyOutlines(bounds);
            _runtime.FloorDecoration.BuildBlackKeys(bounds);
        }

        public void HighlightKeys(IReadOnlyList<int> keyIndices, Color color)
        {
            Color opaqueColor = OpaqueIfTransparent(color);

            for (int i = 0; i < keyIndices.Count; i++)
            {
                int key = keyIndices[i];
                if (key < 0 || key >= _keys.Count || _keys[key].Renderer == null)
                    continue;

                _keys[key].Renderer.color = opaqueColor;
            }
        }

        public void ClearKeyHighlights(IReadOnlyList<int> keyIndices)
        {
            for (int i = 0; i < keyIndices.Count; i++)
            {
                int key = keyIndices[i];
                if (key < 0 || key >= _keys.Count || _keys[key].Renderer == null)
                    continue;

                _keys[key].Renderer.color = _keys[key].BaseColor;
            }
        }

        internal static Color OpaqueIfTransparent(Color color)
        {
            if (color.a < 1f)
                color.a = 1f;

            return color;
        }

        public void DestroyFloor()
        {
            _keys.Clear();
            _runtime.FloorDecoration.Clear();

            if (_floorRoot != null)
                UnityEngine.Object.Destroy(_floorRoot);

            _floorRoot = null;
        }

        private sealed class KeyVisual
        {
            public readonly GameObject GameObject;
            public readonly SpriteRenderer Renderer;
            public readonly Color BaseColor;
            public KeyVisual(GameObject gameObject, SpriteRenderer renderer, Color baseColor)
            {
                GameObject = gameObject;
                Renderer = renderer;
                BaseColor = baseColor;
            }
        }
    }
}
