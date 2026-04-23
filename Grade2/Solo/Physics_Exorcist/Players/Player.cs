using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using _01Scripts.FSM;
using _01Scripts.TurnSystem;
using Chuh007Lib.Dependencies;
using UnityEngine;

namespace _01Scripts.Players
{
    public enum PlayerType
    {
        Search, Battle
    }
    
    public class Player : Entity, IDependencyProvider
    {
        [field:SerializeField] public GameEventChannelSO UIChannel { get; private set; }
        public PlayerType playerType;
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [field: SerializeField] public PlayerBattleInputSO PlayerBattleInput { get; private set; }

        [SerializeField] private StateDataSO[] states;
        
        private EntityStateMachine _stateMachine;

        private bool isUIMode = false;

        [Provide]
        public Player ProvidePlayer() => this;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, states);
            if (playerType == PlayerType.Search)
            {
                PlayerInput.SetCallbacks();
                PlayerBattleInput.RemoveCallbacks();
                PlayerInput.OnESCPressed += HandleESCPressed;

            }
            else if (playerType == PlayerType.Battle)
            {
                PlayerInput.RemoveCallbacks();
                PlayerBattleInput.SetCallbacks();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        protected override void OnDestroy()
        {
            PlayerInput.OnESCPressed -= HandleESCPressed;
            PlayerBattleInput.ClearAllListeners();
            base.OnDestroy();
        }

        private void HandleESCPressed()
        {
            var evt = UIEvents.ESCUIEvent;
            if (!isUIMode)
            {
                evt.isOn = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
            else
            {
                evt.isOn = false;
                Time.timeScale = 1;
                HideMouse();
            }

            isUIMode = !isUIMode;
            UIChannel.RaiseEvent(evt);
        }

        public void ResetPos()
        {
            var controller = GetComponent<CharacterController>();
            controller.enabled = false;
            transform.position = new Vector3(-553f, -24.5f, 347f);
            controller.enabled = true;
        }
        
        public void HideMouse()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        protected override void HandleHit()
        {
        }

        protected override void HandleDead(Entity entity)
        {
            IsDead = true;
            _stateMachine.ChangeState("DEADMOTION");
        }

        private void Start()
        {
            if(playerType == PlayerType.Search) _stateMachine.ChangeState("IDLE");
            if(playerType == PlayerType.Battle) _stateMachine.ChangeState("UIBLOCK");
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        public void ChangeState(string newStatName) => _stateMachine.ChangeState(newStatName);

    }
}