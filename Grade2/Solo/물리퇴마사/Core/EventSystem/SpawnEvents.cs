using _01Scripts.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _01Scripts.Core.EventSystem
{
    public static class SpawnEvents
    {
        public static readonly SpawnAnimationEffect SpawnAnimationEffect = new SpawnAnimationEffect();
        public static readonly SpawnEntityEvent SpawnEntityEvent = new SpawnEntityEvent();
    }

    public class SpawnAnimationEffect : GameEvent
    {
        public PoolItemSO poolType;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Color effectColor;

        public SpawnAnimationEffect Initializer(PoolItemSO poolType, Vector3 position, Quaternion rotation,
            Vector3 scale, Color effectColor)
        {
            this.poolType = poolType;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.effectColor = effectColor;
            return this;
        }
    }

    public class SpawnEntityEvent : GameEvent
    {
        public Entity Entity;
    }
}