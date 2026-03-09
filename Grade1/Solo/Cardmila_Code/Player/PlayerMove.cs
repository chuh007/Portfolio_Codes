using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float maxDis = 20f;
    Vector3 mousePos;
    Camera cam;
    public event Action <Vector3>Move;
    [SerializeField] private GameObject visual;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] MovecardUI movecardUI;
    private void Awake()
    {
        spriteRenderer = visual.GetComponent<SpriteRenderer>();
        animator = visual.GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, transform.forward, maxDis);
            Debug.DrawRay(mousePos, transform.forward * 10, Color.red, 0.3f);
            if (hit)
            {
                if (hit.transform.CompareTag("Left"))
                {
                    animator.SetBool("isMove", true);
                    spriteRenderer.flipX = true;
                    transform.DOMove(transform.position + new Vector3(-1, 0, 0), 0.25f);
                    Move.Invoke(new Vector3(-1,0,0));
                    movecardUI.EndLineWow();
                }  
                else if (hit.transform.CompareTag("Up"))
                {
                    animator.SetBool("isMove", true);
                    transform.DOMove(transform.position + new Vector3(0, 1, 0), 0.25f);
                    Move.Invoke(new Vector3(0, 1, 0));
                    movecardUI.EndLineWow();
                }
                else if (hit.transform.CompareTag("Right"))
                {
                    animator.SetBool("isMove", true);
                    spriteRenderer.flipX = false;
                    transform.DOMove(transform.position + new Vector3(1, 0, 0), 0.25f);
                    Move.Invoke(new Vector3(1, 0, 0));
                    movecardUI.EndLineWow();
                }
                else if (hit.transform.CompareTag("Down"))
                {
                    animator.SetBool("isMove", true);
                    transform.DOMove(transform.position + new Vector3(0, -1, 0), 0.25f);
                    Move.Invoke(new Vector3(0, -1, 0));
                    movecardUI.EndLineWow();
                }
                
                Invoke("MoveEnd", 0.3f);
            }
        }
    }
    private void MoveEnd()
    {
        animator.SetBool("isMove", false);
    }
}
