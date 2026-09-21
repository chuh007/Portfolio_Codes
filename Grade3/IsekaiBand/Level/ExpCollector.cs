using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Test;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Code.LCH._02.Scripts.Level
{
    public class ExpCollector : MonoBehaviour
    {
        private LevelSystemCompo _levelSystemCompo;

        private void Awake()
        {
            _levelSystemCompo = GetComponentInParent<LevelSystemCompo>();
        }

        private void Start()
        {
            ExperienceManager.Instance?.SetPlayer(transform.parent);
            MetaUpgradeContainer.Instance?.ApplyUpgrade();
        }

        private void OnEnable()
        {
            Bus<ExpGainEvent>.OnEvent += OnExpGain;
        }

        private void OnDisable()
        {
            Bus<ExpGainEvent>.OnEvent -= OnExpGain;
        }

        private void OnExpGain(ExpGainEvent e)
        {
            PlayerCommonBuildCompo commonBuild = GetComponentInParent<PlayerCommonBuildCompo>();
            float multiplier = commonBuild != null
                ? commonBuild.ExpGainMultiplier
                : MetaUpgradeRuntime.GetMultiplier(this, MetaUpgradeType.GrowEXP);
            _levelSystemCompo?.LevelSystem.AddExp(e.Amount * multiplier * (TestDoubleMode.Instance.isOnDoubleMode ? 2f : 1f));
        }
    }
}
