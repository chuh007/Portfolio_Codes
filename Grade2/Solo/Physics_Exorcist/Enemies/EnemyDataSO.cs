using System.Collections.Generic;
using _01Scripts.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01Scripts.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "SO/Enemy/Data", order = 0)]
    public class EnemyDataSO : ScriptableObject
    {
        public float health;
        public float damage;
        public float speed;
        public string visualName;
        public string enemyName;
        public Sprite icon;
        [FormerlySerializedAs("isGameEnd")] public bool isBoss = false;
        
        public List<AttackDataSO> attackDataList;
        
    }
}