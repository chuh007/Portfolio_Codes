using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Code.Entities;
using Code.Combats;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FireBullet", story: "[onwer] fire [count] [bullet] to [target] speed: [speed] rotate: [rotate] damage: [damage]", category: "Action", id: "d7f1d19e1abc39ba5744048ed728f088")]
public partial class InitAneFireBulletAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Onwer;
    [SerializeReference] public BlackboardVariable<int> Count;
    [SerializeReference] public BlackboardVariable<Bullet> Bullet;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> Rotate;
    [SerializeReference] public BlackboardVariable<float> Damage;
    protected override Status OnStart()
    {
        Vector2 dir = Target.Value.transform.position - Onwer.Value.transform.position;
        float rotate = -Rotate.Value * (Count.Value / 2);
        float angle = MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        for (int i = 0; i < Count.Value; i++)
        {
            Bullet bullet = UnityEngine.Object.Instantiate(Bullet.Value);
            bullet.transform.position = Onwer.Value.transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle + rotate);
            rotate += Rotate.Value;
            bullet.Initialize(Onwer.Value.GetComponent<Entity>(), Damage.Value);
            bullet.FireBullet(Speed.Value, 3f);
        }
        return Status.Success;
    }

}

