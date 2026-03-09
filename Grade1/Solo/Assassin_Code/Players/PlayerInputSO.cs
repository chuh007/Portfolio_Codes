using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Players
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action OnJumpKeyPressed;
        public event Action OnAttackKeyPressed;
        public event Action OnDashKeyPressed;
        public event Action OnCounterKeyPressed;
        
        public event Action OnESCKeyPressed;
        public event Action<bool> OnOnsetKeyPressed;
        public event Action<bool> OnSkillKeyPressed;
        
        public Vector2 InputDirection { get; private set; }
        
        private Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        public void ClearSubscription()
        {
            OnJumpKeyPressed = null;
            OnAttackKeyPressed = null;
            OnDashKeyPressed = null;
            OnCounterKeyPressed = null;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            InputDirection = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnAttackKeyPressed?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnJumpKeyPressed?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnDashKeyPressed?.Invoke();
        }

        public void OnCounter(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnCounterKeyPressed?.Invoke();
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSkillKeyPressed?.Invoke(true);
            if(context.canceled)
                OnSkillKeyPressed?.Invoke(false);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {

        }

        public void OnOnset(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnOnsetKeyPressed?.Invoke(true);
            if (context.canceled)
                OnOnsetKeyPressed?.Invoke(false);
        }

        public void OnESC(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnESCKeyPressed?.Invoke();
        }
    }
}