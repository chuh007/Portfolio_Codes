using Ami.BroAudio;
using UnityEngine;
using UnityEngine.Serialization;

namespace Work.CHUH._01Scripts.Enemies
{
    public class JaPokEnemyAttackCompo : MeleeEnemyAttackCompo
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private SoundID sound;
        protected override void Attack()
        {
            base.Attack();
            BroAudio.Play(sound);
            Instantiate(bombPrefab, transform.position, Quaternion.identity);
            _owner.OnDeadEvent?.Invoke();
        }
    }
}