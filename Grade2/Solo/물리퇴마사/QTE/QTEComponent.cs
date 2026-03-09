using System.Collections;
using System.Collections.Generic;
using _01Scripts.Entities;
using _01Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

namespace QTESystem
{
    public class QTEComponent : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private GameObject qteParent;
        [SerializeField] private QTEObject qtePrefab;
        [SerializeField] private AudioClip successSound;
        
        [Header("Events")]
        public UnityEvent onSuccess;
        public UnityEvent onFailure;
        
        private Entity _entity;
        private Queue<QTEObject> _qteQueue = new Queue<QTEObject>();
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public IEnumerator QTEStart(int qteCount)
        {
            for (int i = 0; i < qteCount; i++)
            {
                var qte = Instantiate(qtePrefab, qteParent.transform);
                _qteQueue.Enqueue(qte);
                // qte.OnSuccess += DestoryObject;
                qte.OnFail += DestoryObject;
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }
        
        public void HandleQTEPressed()
        {
            var current = _qteQueue.Dequeue();
            if (current.IsQTESuccess())
            {
                QTESuccess();
            }
            else QTEFailure();
        }

        private void DestoryObject(QTEObject obj)
        {
            if(_qteQueue.Count > 0) _qteQueue.Dequeue();
            QTEFailure();
            Destroy(obj);
        }

        private void QTESuccess()
        {
            SoundManager.Instance.PlaySFX(successSound);
            onSuccess.Invoke();
        }

        private void QTEFailure()
        {
            onFailure.Invoke();
        }

    }
}
