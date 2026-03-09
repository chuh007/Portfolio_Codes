using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostTextSet : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    
    public void SetText(int nowValue,int maxValue)
    {
        text.text = $"{nowValue}/{maxValue}";
    }
}
