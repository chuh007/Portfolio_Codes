using System;
using _01Scripts.Core.EventSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _01Scripts.Enemies
{
    public class EnemyActionText : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO enemyChannel;
        [SerializeField] private GameObject actionObj;
        [SerializeField] private TextMeshProUGUI text;

        private void Awake()
        {
            actionObj.transform.localScale = new Vector3(1f, 0, 1f);
            enemyChannel.AddListener<EnemyActionEvent>(HandleActionEvent);
        }

        private void OnDestroy()
        {
            enemyChannel.RemoveListener<EnemyActionEvent>(HandleActionEvent);
        }

        private void HandleActionEvent(EnemyActionEvent evt)
        {
            text.text = evt.description;
            actionObj.transform.DOScaleY(1f, 0.2f);
            DOVirtual.DelayedCall(0.5f, () => actionObj.transform.DOScaleY(0f, 0.2f));
        }
    }
}