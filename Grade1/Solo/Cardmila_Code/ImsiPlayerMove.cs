using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ImsiPlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Camera maincam;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        maincam = Camera.main;
    }
    private void Update()
    {
        Vector2 mouse = maincam.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mouse);
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(x, y);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 a = new Vector3(x, y, 0);
        //    transform.DOMove(transform.position+a, 0.5f);
        //}


    }
}
