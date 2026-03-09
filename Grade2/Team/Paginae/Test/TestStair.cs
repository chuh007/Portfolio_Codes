using System;
using UnityEngine;

namespace Work.CUH.Code.Test
{
    public class TestStair : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            other.transform.position += Vector3.down * 1.25f;
        }
    }
}