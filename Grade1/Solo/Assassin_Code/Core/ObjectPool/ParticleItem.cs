using Code.Core.Pool;
using System;
using System.Collections;
using UnityEngine;

public class ParticleItem : MonoBehaviour,IPoolable
{
    private ParticleSystem _parItem;

    public PollingType PoolType => PollingType.EnemyDeadParticle;

    public GameObject ObjectPrefab => gameObject;

    private void OnEnable()
    {
        _parItem = GetComponentInChildren<ParticleSystem>();
    }

    public void PlayEffect()
    {
        if(_parItem != null)
            _parItem.Play();
        
        StartCoroutine(PushItem());
    }

    IEnumerator PushItem()
    {
        yield return new WaitForSeconds(2.5f);
        PoolManager.Instance.Push(this);
    }

    public void ResetItem()
    {

    }
}
