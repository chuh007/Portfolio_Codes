using UnityEngine;

namespace Code.Core.EventSystems
{
    public static class PlayerEvents
    {
        public static readonly AddEXPEvent AddExpEvent = new AddEXPEvent();
        public static readonly PlayerAttackSuccess PlayerAttackSuccess = new PlayerAttackSuccess();
        public static readonly DashStartEvent DashStartEvent = new DashStartEvent();
        public static readonly DashEndEvent DashEndEvent = new DashEndEvent();
        public static readonly CounterSuccessEvent CounterSuccessEvent = new CounterSuccessEvent();
        public static readonly SkillChargedEvent SkillChargedEvent = new SkillChargedEvent();
        public static readonly SkillChargeEndEvent SkillChargeEndEvent = new SkillChargeEndEvent();
        public static readonly OnsetTargetEvent OnSetTargetEvent = new OnsetTargetEvent();
        
    }

    public class AddEXPEvent : GameEvent
    {
        public int exp;
    }

    public class PlayerAttackSuccess : GameEvent { }
    public class DashStartEvent : GameEvent { }
    public class DashEndEvent : GameEvent { }
    public class CounterSuccessEvent : GameEvent
    {
        public Transform target;
    }

    public class SkillChargedEvent : GameEvent
    {
        public int chargeCount;
    }
    public class SkillChargeEndEvent : GameEvent { }

    public class OnsetTargetEvent : GameEvent
    {
        public Transform target;
    }
}