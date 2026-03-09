using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(LineRenderer))]
public class LightShooter : MonoBehaviour, IInteractable
{
    [Header("Sprite Setting")]
    [SerializeField] private Sprite cantOnSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] protected Sprite onSprite;

    [Header("Light Setting")]
    [SerializeField] private float ShootTime = 1.0f; 
    [SerializeField] private float Colltime = 3.0f;

    public UnityEvent OnEvent;
    public UnityEvent OffEvent;

    private bool isCanLightShoot = true;

    private BoxCollider2D _colider;
    private LineRenderer _lineRenderer;
    private SpriteRenderer spriteRenderer;
    private Light2D _light;
    private int positionCount = 1;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _colider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _light = GetComponentInChildren<Light2D>();
    }
    private void Start()
    {
        _lineRenderer.SetPosition(0, Vector3.zero);
        _light.enabled = false;
    }
    private void FireLight()
    {
        if (!isCanLightShoot) return;
        Debug.Log("발진");
        StartCoroutine(LightColtime());
        _lineRenderer.SetPosition(0, Vector3.zero);
        RaycastHit2D hit = Physics2D.Raycast(transform.position+transform.right, transform.right, 30);
        if (hit.collider != null)
        {
            _lineRenderer.positionCount = positionCount + 1;
            if (hit.collider.transform.CompareTag("Mirror"))
            {
                Debug.Log("거울에맞음");
                _lineRenderer.SetPosition(1, transform.InverseTransformPoint(hit.collider.transform.position));
                hit.collider.transform.GetComponent<MirrorReflection>()?.ReflectionMirror(_lineRenderer, transform.right, this);
                hit.collider.transform.GetComponent<LightSensor>()?.ExecutionEvent();
            }
            else _lineRenderer.SetPosition(1, transform.InverseTransformPoint(hit.point));

        }
    }

    private IEnumerator LightColtime()
    {
        isCanLightShoot = false;
        spriteRenderer.sprite = onSprite;
        _light.enabled = true;
        OnEvent?.Invoke();
        yield return new WaitForSeconds(ShootTime);
        _lineRenderer.positionCount = 1;
        spriteRenderer.sprite = cantOnSprite;
        _light.enabled = false;
        OffEvent?.Invoke();
        yield return new WaitForSeconds(Colltime);
        spriteRenderer.sprite = offSprite;
        isCanLightShoot = true;
        _colider.enabled = true;
    }


    public void Interact()
    {
        SoundManager.Instance.PlaySfx(SFXEnum.LightReflect);
        FireLight();
        _colider.enabled = false;
    }

    public void EndInteract()
    {
    }
}
