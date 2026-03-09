using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPull : Singleton<BulletPull>
{

    public Stack<GameObject> bulletPool = new Stack<GameObject>(); //총알 풀
    private int bulletCount = 10; //처음 생성할 총알 개수
    [SerializeField] GameObject bulletPrefab;
    float desiredAngle;

    private void Start()
    {
        CreateBulletPool();
    }
    public void CreateBulletPool() //풀 생성
    {
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Push(bullet);
        }
    }


    public GameObject GetBulletInPool(Transform _transform,Transform pltr)
    {
        Vector2 plDir = pltr.position - transform.position;
        desiredAngle = Mathf.Atan2(plDir.y, plDir.x) * Mathf.Rad2Deg;
        Quaternion www = Quaternion.AngleAxis(desiredAngle, Vector3.forward);
        GameObject bullet = null;
        if (bulletPool.Count <= 0)
        {
            bullet = Instantiate(bulletPrefab,_transform.position,www);
        }
        else
        {
            bullet = bulletPool.Pop();
            bullet.transform.position = _transform.position;
            bullet.transform.rotation = www;
            bullet.SetActive(true);
        }
        return bullet;
    }

}
