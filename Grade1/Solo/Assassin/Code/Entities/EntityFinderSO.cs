using System;
using UnityEngine;

namespace Code.Entities
{
    [CreateAssetMenu(fileName = "EntityFinder", menuName = "SO/Entity/Finder", order = 0)]
    public class EntityFinderSO : ScriptableObject
    {
        [SerializeField] private string targetTag;
        public Entity target;

        public void SetEntity(Entity entity)
        {
            target = entity;
        }
    }
}