using System.Collections.Generic;
using UnityEngine;

namespace Code.SkillSystem
{
    public abstract class SkillUpgradeDataSO : ScriptableObject
    {
        public string upgradeName;
        public Sprite upgradeIcon;
        
        [TextArea] public string description;
        public int maxUpgradeCount;
        public List<SkillUpgradeDataSO> needUpgradeList = new List<SkillUpgradeDataSO>();
        public List<SkillUpgradeDataSO> dontNeedUpgradeList = new List<SkillUpgradeDataSO>();
        
        public abstract void UpgradeSkill(Skill skill);
        public abstract void RollbackUpgrade(Skill skill);
    }
}
