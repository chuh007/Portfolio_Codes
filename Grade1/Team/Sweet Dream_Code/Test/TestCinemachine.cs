using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class TestCinemachine : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachine;
    [SerializeField] private float camSize = 50f;
    private void Awake()
    {
        Debug.Log(cinemachine.m_Lens.OrthographicSize);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DOTween.To(() => cinemachine.m_Lens.OrthographicSize, x => cinemachine.m_Lens.OrthographicSize = x, camSize, 5);
        }
    }
}
