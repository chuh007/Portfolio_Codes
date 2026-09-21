using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    public static class PersistentResourceIds
    {
        public const string Coin = "coin";
        public const string Note = "note";
    }

    /// <summary>영구 재화와 스킬 트리 진행도의 캐시 및 변경 상태를 관리한다.</summary>
    public static class PersistentProgressStore
    {
        private static PersistentProgressData _data;
        private static bool _isLoaded;
        private static bool _isDirty;

        public static bool HasUnsavedChanges => _isDirty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState() => ResetCachedData();

        internal static void ResetCachedData()
        {
            _data = null;
            _isLoaded = false;
            _isDirty = false;
        }

        public static bool TryGetResourceAmount(string resourceId, out int amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                amount = 0;
                return false;
            }
            return GetData().TryGetResourceAmount(resourceId, out amount);
        }

        public static void SetResourceAmount(string resourceId, int amount)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource ID must not be empty.", nameof(resourceId));
            if (GetData().SetResourceAmount(resourceId, amount))
                _isDirty = true;
        }

        public static IReadOnlyList<string> GetActivatedTreeNodeIds()
            => new List<string>(GetData().activatedTreeNodeIds);

        public static void SetActivatedTreeNodeIds(IEnumerable<string> nodeIds)
        {
            var normalizedIds = new List<string>();
            var uniqueIds = new HashSet<string>(StringComparer.Ordinal);
            if (nodeIds != null)
            {
                foreach (string nodeId in nodeIds)
                {
                    if (string.IsNullOrWhiteSpace(nodeId) || !uniqueIds.Add(nodeId)) continue;
                    normalizedIds.Add(nodeId);
                }
            }

            normalizedIds.Sort(StringComparer.Ordinal);
            PersistentProgressData data = GetData();
            if (ListsMatch(data.activatedTreeNodeIds, normalizedIds)) return;

            data.activatedTreeNodeIds = normalizedIds;
            _isDirty = true;
        }

        public static void Flush()
        {
            if (!_isDirty) return;

            try
            {
                PersistentProgressStorage.Save(GetData());
                _isDirty = false;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[PersistentProgressStore] 저장에 실패했습니다.\n{exception}");
            }
        }

        private static PersistentProgressData GetData()
        {
            if (_isLoaded) return _data;

            _isLoaded = true;
            _data = PersistentProgressStorage.Load();
            _data.Normalize();
            return _data;
        }

        private static bool ListsMatch(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int i = 0; i < left.Count; i++)
            {
                if (!string.Equals(left[i], right[i], StringComparison.Ordinal)) return false;
            }
            return true;
        }
    }
}
