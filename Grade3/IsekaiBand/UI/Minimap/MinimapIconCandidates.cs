using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapIconCandidates
    {
        private readonly Dictionary<Transform, SpriteRenderer> _iconCandidates = new();

        public IReadOnlyDictionary<Transform, SpriteRenderer> Gather(LayerMask iconLayerMask)
        {
            _iconCandidates.Clear();

            SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            foreach (SpriteRenderer renderer in renderers)
            {
                if (!IsUsableRenderer(renderer)) continue;

                Transform owner = FindLayerOwner(renderer.transform, iconLayerMask);
                if (owner == null) continue;

                if (!_iconCandidates.TryGetValue(owner, out SpriteRenderer current)
                    || CompareRendererPriority(renderer, current) > 0)
                {
                    _iconCandidates[owner] = renderer;
                }
            }
            return _iconCandidates;
        }

        private static Transform FindLayerOwner(Transform rendererTransform, LayerMask iconLayerMask)
        {
            for (Transform current = rendererTransform; current != null; current = current.parent)
            {
                if ((iconLayerMask.value & (1 << current.gameObject.layer)) != 0)
                    return current;
            }

            return null;
        }

        public static bool IsUsableRenderer(SpriteRenderer renderer)
        {
            return renderer != null
                   && renderer.enabled
                   && renderer.gameObject.activeInHierarchy
                   && renderer.sprite != null
                   && renderer.color.a > 0.01f;
        }

        private static int CompareRendererPriority(SpriteRenderer a, SpriteRenderer b)
        {
            int layerCompare = SortingLayer.GetLayerValueFromID(a.sortingLayerID)
                               .CompareTo(SortingLayer.GetLayerValueFromID(b.sortingLayerID));
            if (layerCompare != 0)
                return layerCompare;

            return a.sortingOrder.CompareTo(b.sortingOrder);
        }
    }
}
