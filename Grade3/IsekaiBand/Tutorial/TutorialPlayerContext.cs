using System.Collections;
using _Code.LCH._02.Scripts.Level;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.EntityPlus;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialPlayerContext
    {

        private readonly TutorialSequenceController _settings;
        public Player Player { get; }
        public PlayerMovementCompo Movement { get; }
        public EntityHealth Health { get; }
        public PlayerAttackCompo Attack { get; }
        public LevelSystemCompo LevelSystem { get; }
        public WeaponBuildManager BuildManager { get; }
        public CombineWeaponController Combination { get; }

        public TutorialPlayerContext(TutorialSequenceController settings)
        {
            _settings = settings;
            Player = UnityEngine.Object.FindAnyObjectByType<Player>();
            if (Player == null) return;
            Movement = Player.GetComponent<PlayerMovementCompo>();
            Health = Player.GetComponentInChildren<EntityHealth>(true);
            Attack = Player.GetComponent<PlayerAttackCompo>();
            LevelSystem = Player.GetComponent<LevelSystemCompo>();
            Combination = Player.GetComponent<CombineWeaponController>();
            BuildManager = WeaponBuildManager.GetOrCreate(Attack);
        }

        public bool IsReady => Player != null && Movement != null && Health != null && Attack != null
                               && LevelSystem?.LevelSystem != null && BuildManager != null && Combination != null;

        public bool IsDashing => Movement.IsDashing || Player.IsDash;

        public bool GrantCompletedInstrument(WeaponType weaponType)
        {
            if (Attack == null || BuildManager == null)
                return false;

            if (!Attack.HasWeapon(weaponType) && !BuildManager.AcquireInstrument(weaponType))
                return false;

            foreach (InstrumentPartDefinition part in InstrumentPartRules.GetParts(weaponType))
            {
                if (BuildManager.HasInstrumentPart(weaponType, part.PartIndex))
                    continue;

                if (!BuildManager.ApplyInstrumentPart(
                        weaponType,
                        part.PartIndex,
                        notifyPresentation: false))
                    return false;
            }

            return BuildManager.IsInstrumentComplete(weaponType);
        }

        public IEnumerator WaitForMovement()
        {
            Vector2 previousPosition = Player.transform.position;
            float travelledDistance = 0f;

            while (travelledDistance < _settings.RequiredMovementDistance)
            {
                yield return null;
                Vector2 currentPosition = Player.transform.position;
                travelledDistance += Mathf.Min(Vector2.Distance(previousPosition, currentPosition), 1f);
                previousPosition = currentPosition;
            }
        }
    }
}
