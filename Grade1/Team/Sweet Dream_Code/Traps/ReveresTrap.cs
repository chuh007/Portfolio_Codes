using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReveresTrap : MonoBehaviour
{
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private float returnTime = 3f;
    private bool isFilp = false;
    private bool isCol = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&!isCol)
        {
            StartCoroutine(Reveres());
        }
    }
    private IEnumerator Reveres()
    {
        isCol = true;
        yield return new WaitForSeconds(waitTime);
        transform.DOShakePosition(waitTime, new Vector3(0.1f, 0, 0), 15, 50f, false);
        yield return new WaitForSeconds(shakeTime);
        ReveresGround(isFilp);
        yield return new WaitForSeconds(returnTime);
        ReveresGround(isFilp);
        isCol = false;
    }

    private void ReveresGround(bool filp)
    {
        
        if(isFilp == false)
        {
            transform.DORotate(new Vector3(0,0,180f),0.2f,RotateMode.Fast);
        }
        else
        {
            transform.DORotate(new Vector3(0, 0, 0), 0.2f, RotateMode.Fast);

        }
        isFilp = !filp;

    }

}
