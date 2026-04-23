using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01Scripts.Players
{
    [CreateAssetMenu(fileName = "PlayerBattleInputSO", menuName = "SO/PlayerBattleInputSO", order = 0)]
    public class PlayerBattleInputSO : ScriptableObject, Controls.IBattlePlayerActions
    {
        public event Action OnAttackKeyPressed;
        public event Action OnQTEKeyPressed;
        public event Action OnBlockKeyPressed;
        public event Action OnItemKeyPressed;
        public event Action OnCancelOrESCKeyPressed;
        public event Action OnSelect1KeyPressed;
        public event Action OnSelect2KeyPressed;
        public event Action OnSelect3KeyPressed;

        private Controls _controls;

        private void OnEnable()
        {
            if(_controls == null)
            {
                _controls = new Controls();
                _controls.BattlePlayer.SetCallbacks(this);
            }
            _controls.Enable();
            
        }
        
        private void OnDisable()
        {
            _controls.Player.Disable();
            _controls.BattlePlayer.Disable();
            _controls.UI.Disable();
        }
        
        public void RemoveCallbacks()
        {
            _controls.BattlePlayer.Disable();
        }
        
        public void SetCallbacks()
        {
            _controls.BattlePlayer.Enable();
        }
        
        public void OnAttackQTE(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnQTEKeyPressed?.Invoke();
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnBlockKeyPressed?.Invoke();
        }

        public void OnSelect1(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelect1KeyPressed?.Invoke();
        }

        public void OnSelect2(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelect2KeyPressed?.Invoke();
        }

        public void OnSelect3(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelect3KeyPressed?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackKeyPressed?.Invoke();
            }
        }
        
        public void OnItem(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnItemKeyPressed?.Invoke();
        }

        public void OnCancelOrESC(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnCancelOrESCKeyPressed?.Invoke();
        }

        public void OnMouse(InputAction.CallbackContext context)
        {
            
        }

        public void ClearAllListeners()
        {
            OnAttackKeyPressed = null;
            OnQTEKeyPressed = null;
            OnBlockKeyPressed = null;
            OnItemKeyPressed = null;
            OnCancelOrESCKeyPressed = null;
            OnSelect1KeyPressed = null;
            OnSelect2KeyPressed = null;
            OnSelect3KeyPressed = null;
        }
    }
}