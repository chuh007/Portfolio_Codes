using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("ISwitchable »ó¼Ó")]
    public MonoBehaviour ActiveObj;

    public UnityEvent OnEvent;
    public UnityEvent OffEvent;

    [SerializeField] private Sprite _lever;

    private ISwitchable active;
    private SpriteRenderer _sprite;

    private void OnValidate()
    {
        if (ActiveObj != null && ActiveObj as ISwitchable == null)
        {
            ActiveObj = null;
            Debug.LogWarning("This Object is not succession ISwitchble. Put in ISwitchble Obj");
        }
    }
    private void Awake()
    {
        try
        {
            active = (ISwitchable)ActiveObj;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ActiveObj is not succession ISwitchble. Message : {ex}");
        }
        _sprite = GetComponent<SpriteRenderer>();
    }
    public void Interact()
    {
        _sprite.sprite = _lever;
        StartCoroutine(WaitAndOff());
        active.Activate();
    }
    public void EndInteract()
    {

    }

    private IEnumerator WaitAndOff()
    {
        OnEvent?.Invoke();
        yield return new WaitForSeconds(2);
        OffEvent?.Invoke();
    }
}
