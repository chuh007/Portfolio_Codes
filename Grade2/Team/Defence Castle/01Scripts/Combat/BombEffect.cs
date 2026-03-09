using System;
using System.Collections;
using UnityEngine;

namespace Work.CHUH._01Scripts.Combat
{
    public class BombEffect : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(BombDelete());
        }

        private IEnumerator BombDelete()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}