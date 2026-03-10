using Code.Combats;
using Code.Entities;
using Code.Players;
using System.Collections;
using UnityEngine;


public class Bullet : MonoBehaviour
{

    private Rigidbody2D _rbCompo;
    private Entity _owner;
    private float _damage;

    public void Initialize(Entity owner, float damage)
    {
        _owner = owner;
        _damage = damage;
    }

    private void Awake()
    {
        _rbCompo = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.ApplyDamage(_damage, transform.right, new Vector2(3f, 5f), false, _owner);
            Destroy(gameObject);
        }
    }


    public void FireBullet(float speed, float lifeTime)
    {
        _rbCompo.AddForce(transform.right * speed, ForceMode2D.Impulse);
        StartCoroutine(DestroyCorutine(lifeTime));
    }

    private IEnumerator DestroyCorutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

