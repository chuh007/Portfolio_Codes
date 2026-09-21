using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class BandRuntimeObjects
    {
        private readonly List<GameObject> _objects = new();

        public int Count => _objects.Count;

        public void Add(GameObject obj) => _objects.Add(obj);
        public void Remove(GameObject obj) => _objects.Remove(obj);

        public void RemoveDestroyed(bool includeInactive = true)
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                if (_objects[i] == null || includeInactive && !_objects[i].activeInHierarchy)
                    _objects.RemoveAt(i);
            }
        }

        public void MakeRoom(int capacity)
        {
            while (_objects.Count >= capacity)
            {
                GameObject oldest = _objects[0];
                _objects.RemoveAt(0);
                if (oldest != null) Object.Destroy(oldest);
            }
        }

        public void ReturnToPool(bool reverseOrder = false, bool activeOnly = false)
            => Release(obj => ProjectilePool.Push(obj), reverseOrder, activeOnly);

        public void DestroyAll(bool reverseOrder = false)
            => Release(obj => Object.Destroy(obj), reverseOrder, false);

        private void Release(System.Action<GameObject> release, bool reverseOrder, bool activeOnly)
        {
            // 반환 콜백이 소유 목록을 수정해도 누락 없이 지정된 순서로 반환한다.
            GameObject[] objects = _objects.ToArray();
            _objects.Clear();
            for (int offset = 0; offset < objects.Length; offset++)
            {
                int index = reverseOrder ? objects.Length - 1 - offset : offset;
                GameObject obj = objects[index];
                if (obj != null && (!activeOnly || obj.activeInHierarchy)) release(obj);
            }
        }
    }
}
