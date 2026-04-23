using System;
using System.Collections;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using _01Scripts.FSM;
using _01Scripts.Players;
using _01Scripts.TurnSystem;
using Chuh007Lib.Dependencies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01Scripts.Enemies
{
    public class Enemy : Entity
    {
        [Inject, HideInInspector] public Player target;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO spawnChannel;
        public GameEventChannelSO enemyChannel;

        [SerializeField] private StateDataSO[] states;
        
        private EntityStateMachine _stateMachine;
        
        private bool _isGameEndEnemy = false;

        public bool IsBoss => _isGameEndEnemy;
        
        protected override void Awake()
        {
            base.Awake();
            SpawnEventRaise();
            _stateMachine = new EntityStateMachine(this, states);
        }

        protected override void HandleHit()
        {
            _stateMachine.ChangeState("HIT");
            
        }

        protected override void HandleDead(Entity entity)
        {
            IsDead = true;
            _stateMachine.ChangeState("DEAD");
            if (_isGameEndEnemy)
            {
                uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
                StartCoroutine(StartFade());
            }
        }

        private IEnumerator StartFade()
        {
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = false;
            fadeEvt.fadeTime = 0.5f;
            yield return new WaitForSeconds(0.5f);
            uiChannel.RaiseEvent(fadeEvt);
        }

        private void HandleFadeComplete(FadeCompleteEvent obj)
        {
            SoundManager.Instance.StopBGM();
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete);
            SceneManager.LoadScene("EndScene");
        }
        
        public void SetBoss()
        {
            _isGameEndEnemy = true;
        }
        
        public void SpawnEventRaise()
        {
            var evt = SpawnEvents.SpawnEntityEvent;
            evt.Entity = this;
            spawnChannel.RaiseEvent(evt);
        }

        private void Start()
        {
            _stateMachine.ChangeState("IDLE");
        }
        
        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        
        public void ChangeState(string newStatName) => _stateMachine.ChangeState(newStatName);
        
    }
}