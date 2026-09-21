using Unity.Behavior;

namespace _Work.CHUH.Code.BT
{
    [BlackboardEnum]
    public enum BTEnemyState
    {
        WAIT = 0,
        CHASE = 1,
        ATTACK_WARNING = 2,
        ATTACK = 3,
        DEATH = 4
    }
}
