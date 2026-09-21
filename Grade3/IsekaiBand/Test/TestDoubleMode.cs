using System;
using _Code.LCH._02.Scripts.Level;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Test
{
    public class TestDoubleMode : MonoBehaviour
    {
        public static TestDoubleMode Instance;
        public bool isOnDoubleMode;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }
        
    }
}