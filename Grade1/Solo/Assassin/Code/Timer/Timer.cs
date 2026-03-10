using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Timer
{
    public class Timer : MonoBehaviour
    {
        public UnityEvent TimeOverEvent;

        [SerializeField] private TextMeshProUGUI UIText;
        [SerializeField] private float maxTime = 60f;
        private float leftTime;

        private void Awake()
        {
            leftTime = maxTime;
        }

        private void Update()
        {
            leftTime = Mathf.Max(leftTime - Time.deltaTime, 0);
            UIText.text = leftTime.ToString("F2"); 
            if(leftTime <= 0) TimeOverEvent?.Invoke();
        }
    }
}

