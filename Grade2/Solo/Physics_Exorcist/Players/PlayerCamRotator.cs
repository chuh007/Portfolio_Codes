using System;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerCamRotator : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform cameraTrm;
        [SerializeField] private float rotationSpeedMulti = 1f;
        
        private readonly float _widthSpeed = Screen.width * 0.01f;
        private readonly float _heightSpeed = Screen.height * 0.01f;
        
        private Entity _entity;
        private Vector2 _mouseDirection;
        private Vector2 _playerRotation;
        private Vector2 _camRotation;


        public void Initialize(Entity entity)
        {
            _entity = entity;
            _playerRotation = entity.transform.rotation.eulerAngles;
            _camRotation = cameraTrm.rotation.eulerAngles;
        }

        public void SetMouseDirection(Vector2 mouseInput)
        {
            _mouseDirection = new Vector2(mouseInput.x, mouseInput.y);
        }

        private void Update()
        {
            _playerRotation.y += _mouseDirection.x * _widthSpeed * Time.deltaTime * rotationSpeedMulti;
            _camRotation.y = _playerRotation.y;
            _camRotation.x -= _mouseDirection.y * _heightSpeed * Time.deltaTime * rotationSpeedMulti;
            _camRotation.x = Mathf.Clamp(_camRotation.x, -85f, 60f);
            _entity.transform.rotation = Quaternion.Euler(_playerRotation);
            cameraTrm.rotation = Quaternion.Euler(_camRotation);
        }
    }
}