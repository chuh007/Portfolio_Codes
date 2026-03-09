using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    BulletPull bullet;
    private void Awake()
    {
        bullet = BulletPull.Instance;
    }
    public void DoMovePl(Transform tr)
    {
        StartCoroutine(Moveing(tr));
    }
    private IEnumerator Moveing(Transform tr)
    {
        transform.DOMove(tr.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        bullet.bulletPool.Push(gameObject);
        gameObject.SetActive(false);
    }
}
