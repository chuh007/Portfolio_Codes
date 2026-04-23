using System;
using UnityEngine;

namespace _01Scripts.UI
{
    public class ObjectUI : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
        
    }
}
