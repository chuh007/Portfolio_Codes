using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    [Serializable]
    internal sealed class ResourceAmountSaveData
    {
        public string id;
        public int amount;

        public ResourceAmountSaveData(string id, int amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }

    [Serializable]
    internal sealed class PersistentProgressData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public List<ResourceAmountSaveData> resources = new List<ResourceAmountSaveData>();
        public List<string> activatedTreeNodeIds = new List<string>();

        public void Normalize()
        {
            version = CurrentVersion;
            resources ??= new List<ResourceAmountSaveData>();
            activatedTreeNodeIds ??= new List<string>();

            var resourceIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = resources.Count - 1; i >= 0; i--)
            {
                ResourceAmountSaveData resource = resources[i];
                if (resource == null || string.IsNullOrWhiteSpace(resource.id) ||
                    !resourceIds.Add(resource.id))
                {
                    resources.RemoveAt(i);
                    continue;
                }

                resource.amount = Mathf.Max(0, resource.amount);
            }

            var nodeIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = activatedTreeNodeIds.Count - 1; i >= 0; i--)
            {
                string nodeId = activatedTreeNodeIds[i];
                if (string.IsNullOrWhiteSpace(nodeId) || !nodeIds.Add(nodeId))
                    activatedTreeNodeIds.RemoveAt(i);
            }
        }

        public bool TryGetResourceAmount(string resourceId, out int amount)
        {
            foreach (ResourceAmountSaveData resource in resources)
            {
                if (resource.id != resourceId) continue;

                amount = resource.amount;
                return true;
            }

            amount = 0;
            return false;
        }

        public bool SetResourceAmount(string resourceId, int amount)
        {
            amount = Mathf.Max(0, amount);
            foreach (ResourceAmountSaveData resource in resources)
            {
                if (resource.id != resourceId) continue;
                if (resource.amount == amount) return false;

                resource.amount = amount;
                return true;
            }

            resources.Add(new ResourceAmountSaveData(resourceId, amount));
            return true;
        }
    }
}
