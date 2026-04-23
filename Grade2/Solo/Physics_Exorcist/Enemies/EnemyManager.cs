using System;
using System.Collections;
using System.Collections.Generic;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _01Scripts.Enemies
{
    [DefaultExecutionOrder(-1)]
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO spawnChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        
        public List<Enemy> enemies;
        
        private void Awake()
        {
            spawnChannel.AddListener<SpawnEntityEvent>(HandleEnemySpawn);
        }

        private void OnDestroy()
        {
            spawnChannel.RemoveListener<SpawnEntityEvent>(HandleEnemySpawn);
        }
        
        private void HandleEnemySpawn(SpawnEntityEvent evt)
        {
            if(evt.Entity as Enemy)
                enemies.Add(evt.Entity as Enemy);
            evt.Entity.OnDead.AddListener(HandleEnemyDead);
        }

        private void HandleEnemyDead(Entity entity)
        {
            Enemy enemy = entity as Enemy;
            if(enemy.IsBoss) return;
            enemies.Remove(enemy);
            if (enemies.Count == 0)
            {
                uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
                StartCoroutine(GotoBattle());
            }
        }
        
        private IEnumerator GotoBattle()
        {
            yield return new WaitForSeconds(1f);
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = false;
            fadeEvt.fadeTime = 0.5f;
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.StopBGM();
            uiChannel.RaiseEvent(fadeEvt);

        }

        private void HandleFadeComplete(FadeCompleteEvent obj)
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete); //이벤트 제거후 씬변환.
            SceneManager.LoadScene("SerachScene");
        }
    }
}