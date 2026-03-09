using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Multiplexer : MonoBehaviour, ISwitchable
{
    public List<MonoBehaviour> switchList = new List<MonoBehaviour>();
    public UnityEvent ActiveObj;
    public void Activate()
    {
        foreach (var t in switchList)
        {
            ISwitchable switchable = t as ISwitchable;
            if (switchable != null)
                switchable.Activate();
        }
        ActiveObj?.Invoke();
    }

    public void Deactivate()
    {
    }
}
