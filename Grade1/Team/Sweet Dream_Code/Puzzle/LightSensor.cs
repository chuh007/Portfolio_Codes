using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightSensor : MonoBehaviour
{
    [SerializeField] private Sprite OffSprite;
    [SerializeField] private Sprite OnSprite;

    private SpriteRenderer _spriteRenderer;
    public MonoBehaviour ActiveObj;
    private ISwitchable active;

    private void OnValidate()
    {
        if (ActiveObj !=null && ActiveObj as ISwitchable == null)
        {
            ActiveObj = null;
            Debug.LogWarning("This Object is not succession ISwitchble. Put in ISwitchble Objexct");
        }
    }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        try
        {
            active = (ISwitchable)ActiveObj;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ActiveObj is not succession ISwitchble. Message : {ex}");
        }
    }
    public void ExecutionEvent()
    {
        active?.Activate();
        _spriteRenderer.sprite = OnSprite;
    }

}