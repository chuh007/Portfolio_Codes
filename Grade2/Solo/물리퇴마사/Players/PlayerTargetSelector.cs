using System;
using System.Collections.Generic;
using System.Linq;
using _01Scripts.Core.EventSystem;
using _01Scripts.Enemies;
using _01Scripts.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace _01Scripts.Players
{
    public class PlayerTargetSelector : MonoBehaviour, IEntityComponent
    {
        public Entity CurrentTarget { get; private set; }
        
        [SerializeField] private GameEventChannelSO spawnChannel;

        private List<Enemy> _targets = new List<Enemy>();
        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        private void Awake()
        {
            spawnChannel.AddListener<SpawnEntityEvent>(HandleSpawnEntity);
        }

        private void HandleSpawnEntity(SpawnEntityEvent obj)
        {
            _targets.Add(obj.Entity as Enemy);
            CurrentTarget = _targets[0];
        }

        public void ReGetEntity()
        {
            _targets = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
            CurrentTarget = _targets[0];
        }
        
        private void OnDestroy()
        {
            spawnChannel.RemoveListener<SpawnEntityEvent>(HandleSpawnEntity);
        }
    }
}