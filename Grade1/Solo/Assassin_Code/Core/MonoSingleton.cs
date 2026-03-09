using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _isDestoryed;

    public static T Instance
    {
        get
        {
            if (_isDestoryed)
                _instance = null;
            if(_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<T>();
                if (_instance == null)
                    Debug.LogError($"{typeof(T).Name} singleton is not exist");
                else _isDestoryed = false;
            }

            return _instance;
        }
    }
    private void OnDisable()
    {
        _isDestoryed = true;
    }
}
