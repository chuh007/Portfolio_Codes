using Code.Animators;
using UnityEngine;

namespace Code.Entities
{
    public class EntityRenderer : MonoBehaviour, IEntityComponent
    {
        public SpriteRenderer SpriteRenderer { get; private set; }
        
        [field: SerializeField] public float FacingDirection { get; private set; } = 1f;
        [SerializeField] private AnimParamSO yVelocityParam;
        
        private Entity _entity;
        private Animator _animator;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetParam(AnimParamSO param, bool value) => _animator.SetBool(param.hashValue, value);
        public void SetParam(AnimParamSO param, float value) => _animator.SetFloat(param.hashValue, value);
        public void SetParam(AnimParamSO param, int value) => _animator.SetInteger(param.hashValue, value);
        public void SetParam(AnimParamSO param) => _animator.SetTrigger(param.hashValue);

        public void HandleVelocityChange(Vector2 movement)
        {
            if(yVelocityParam != null)
                SetParam(yVelocityParam, movement.y);
        }
        
        #region Flip Controller
        
        public void Flip()
        {
            FacingDirection *= -1;
            _entity.transform.Rotate(0, 180f, 0);
        }

        public void FlipController(float xVelocity)
        {
            float xMove = Mathf.Approximately(xVelocity, 0) ? 0 : Mathf.Sign(xVelocity);
            if (Mathf.Abs(xMove + FacingDirection) < 0.5f) //바라보는 방향과 진행방향이 다르다면 플립
            {
                Flip();
            }
                
        }

        #endregion
    }
}