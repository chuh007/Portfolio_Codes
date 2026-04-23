using System;
using _01Scripts.Entities;
using _01Scripts.Interact;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerInteract : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform cameraTrm;
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private AudioClip catchSound;
        
        private Player _player;
        private GameObject _targetObj;
        private IInteractable _target;
        private string _targetName;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        private void Update()
        {
            Physics.Raycast(cameraTrm.position, cameraTrm.forward, out RaycastHit hit, 3f, whatIsTarget);
            if (hit.transform == null)
            {
                if(_targetObj != null)
                    _targetObj.layer = LayerMask.NameToLayer("InteractObject");
                nameText.gameObject.SetActive(false);
                _targetObj = null;
                _target = null;
                return;
            }
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                _targetObj = hit.transform.gameObject;
                _targetObj.layer = LayerMask.NameToLayer("Outlined");
                _target = interactable;
                _targetName = interactable.Name;
                nameText.gameObject.SetActive(true);
                nameText.text = _targetName;
            }
        }

        public void Interact()
        {
            _player.ChangeState("INTERACT");
            DOVirtual.DelayedCall(0.25f, () =>
            {
                SoundManager.Instance.PlaySFX(catchSound);
                _target?.Interact();
            });
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if(cameraTrm == null) return;
            Gizmos.DrawRay(cameraTrm.position, cameraTrm.forward * 5);
        }
    }
}