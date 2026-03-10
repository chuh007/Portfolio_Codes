using Code.SkillSystem;
using UnityEngine;

namespace Code.Test
{
    public class SkillUpgradeTest : MonoBehaviour
    {
        [SerializeField] private Skill _targetSkill;
        [SerializeField] private SkillPerkUpgradeSO _skillUpgradeData;


        [ContextMenu("Upgrade Skill")]
        private void UpgradeSkill()
        {
            _skillUpgradeData.UpgradeSkill(_targetSkill);
        }
        
        [ContextMenu("RollBack Skill")]
        private void RollBackSkill()
        {
            _skillUpgradeData.RollbackUpgrade(_targetSkill);
        }
    }
}