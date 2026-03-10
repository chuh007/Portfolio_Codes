using System;
using Code.Combats;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem.IceBalls
{
    public class IceBall : MonoBehaviour
    {
        [SerializeField] private float fireSpeed, lifeTime;
        [SerializeField] private ParticleSystem explosionPrefab; //나중에 풀매니징으로 해도 되고.
        [SerializeField] private AttackDataSO attackData;
        [SerializeField] private DamageCaster damageCaster;
        
        private Rigidbody2D _rbCompo;
        private float _currentTime;
        private float _damage;
        private Entity _owner;
        private IceBallSkill _skill;
        private float _guideTime;
        
        public Transform guideTargetTrm;

        private void Awake()
        {
            _rbCompo = GetComponent<Rigidbody2D>();
        }

        public void SetUpAndFire(IceBallSkill skill, Vector3 position, Vector2 direction, float damage, Entity owner)
        {
            _skill = skill;
            _damage = damage;
            _owner = owner;
            transform.position = position;
            damageCaster.InitCaster(owner);
            
            FireProjectile(direction);
        }

        private void FireProjectile(Vector2 direction)
        {
            _currentTime = 0;
            float speed = guideTargetTrm != null ? fireSpeed * 0.2f : fireSpeed;
            
            _rbCompo.linearVelocity = direction.normalized * speed;
        }

        private void FixedUpdate()
        {
            if (guideTargetTrm != null)
            {
                _guideTime += Time.fixedDeltaTime;
                Vector2 direction = guideTargetTrm.position - transform.position;
                _rbCompo.AddForce(direction.normalized * (_skill.guideForce * _guideTime), ForceMode2D.Force);
            }

            transform.right = _rbCompo.linearVelocity.normalized;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= lifeTime)
            {
                Explosion();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            damageCaster.CastDamage(_damage, attackData.knockBackForce, attackData.isPowerAttack);
            Explosion();
            Destroy(gameObject);
        }

        private void Explosion() //메구밍
        {
            ParticleSystem effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            effect.Play();
        }
    }
}