using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class LargeDecorationPool
    {
        private readonly Dictionary<GameObject, Stack<GameObject>> _largeDecorationPools = new();
        private readonly InfiniteMapDecorationSurface _surface;
        private readonly int _wallLayer;
        private readonly int _entitySortingLayerId;
        private Transform _root => _surface.Root;

        public LargeDecorationPool(InfiniteMapDecorationSurface surface)
        {
            _surface = surface;
            int layer = LayerMask.NameToLayer("Wall");
            _wallLayer = layer >= 0 ? layer : 0;
            _entitySortingLayerId = SortingLayer.NameToID("Entity");
        }

        public GameObject Acquire(GameObject prefab)
        {
            if (!_largeDecorationPools.TryGetValue(prefab, out Stack<GameObject> pool))
            {
                pool = new Stack<GameObject>();
                _largeDecorationPools.Add(prefab, pool);
            }

            GameObject instance;
            if (pool.Count > 0)
            {
                instance = pool.Pop();
            }
            else
            {
                instance = UnityEngine.Object.Instantiate(prefab, _root);
                instance.name = prefab.name;
                ConfigureLargeDecoration(instance);
            }

            instance.SetActive(true);
            return instance;
        }

        private void ConfigureLargeDecoration(GameObject instance)
        {
            foreach (Transform child in instance.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = _wallLayer;

            SortingGroup sortingGroup = instance.GetComponent<SortingGroup>();
            if (sortingGroup == null)
                sortingGroup = instance.AddComponent<SortingGroup>();

            sortingGroup.sortingLayerID = _entitySortingLayerId;
            sortingGroup.sortingOrder = 0;

            Collider2D[] colliders = instance.GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D decorationCollider in colliders)
                decorationCollider.isTrigger = false;

            if (colliders.Length == 0)
                Debug.LogWarning($"[StageDecoration] {instance.name} needs a Collider2D.");
        }

        public void ApplyTransform(
            GameObject instance,
            LargeDecorationPlacement placement)
        {
            Transform instanceTransform = instance.transform;
            Transform prefabTransform = placement.Prefab.transform;
            Vector3 localScale = prefabTransform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (placement.FlipX ? -1f : 1f);
            instanceTransform.SetLocalPositionAndRotation(
                new Vector3(placement.Cell.x + 0.5f, placement.Cell.y + 0.5f, 0f),
                prefabTransform.localRotation);
            instanceTransform.localScale = localScale;
        }

        public void Release(ActiveLargeDecoration decoration)
        {
            if (decoration.Instance == null)
                return;

            decoration.Instance.SetActive(false);
            decoration.Instance.transform.SetParent(_root, false);

            if (!_largeDecorationPools.TryGetValue(decoration.Prefab, out Stack<GameObject> pool))
            {
                pool = new Stack<GameObject>();
                _largeDecorationPools.Add(decoration.Prefab, pool);
            }

            pool.Push(decoration.Instance);
        }
    }
}
