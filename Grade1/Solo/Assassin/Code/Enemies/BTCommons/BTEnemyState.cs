using Unity.Behavior;

namespace Code.Enemies.BTCommons
{
    [BlackboardEnum]
    public enum BTEnemyState
    {
        PATROL, REACT, CHASE, ATTACK, STUN, HIT, DEATH
    }
}