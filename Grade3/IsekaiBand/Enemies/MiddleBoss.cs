using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    public class MiddleBoss : FSMEnemy
    {
        [SerializeField] private bool scaleHealthByEncounterCount = true;

        public override bool IsBoss => true;
        public bool ScaleHealthByEncounterCount => scaleHealthByEncounterCount;

        protected void Start()
        {
            _stateMachine.ChangeState(StateName.Move);
        }

        protected override void HandleHit()
        {
        }
    }
}
