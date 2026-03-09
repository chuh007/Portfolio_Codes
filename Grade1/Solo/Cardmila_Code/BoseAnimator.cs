using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoseAnimator : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void MoveAnim()
    {
        StartCoroutine(Move());
    }
    public void AttackAni()
    {
        StartCoroutine(Attack());
    }
    private IEnumerator Move()
    {
        animator.SetBool("Ismove",true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Ismove", false);
    }
    private IEnumerator Attack()
    {
        animator.SetBool("IsAttack", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("IsAttack", false);
    }
}
