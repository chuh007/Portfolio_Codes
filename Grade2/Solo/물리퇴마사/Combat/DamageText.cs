using TMPro;
using UnityEngine;

namespace _01Scripts.Combat
{
    public class DamageText : MonoBehaviour
    {
        [Header("Damage Text Settings")]
        [SerializeField] private float moveHeight = 1.5f;
        [SerializeField] private float duration = 0.8f;
        [SerializeField] private float missTime = 0.3f;

        private TextMeshProUGUI _damageText;
        private Vector3 beforePos;
        private float _startTime;

        private void Awake()
        {
            _damageText = GetComponent<TextMeshProUGUI>();
        }

        public void SetDamageAndPos(float damage, Vector3 pos)
        {
            if (_damageText == null) return;

            _damageText.text = damage.ToString();
            beforePos = pos + Random.insideUnitSphere * 0.5f + Vector3.up * 0.75f;
            _startTime = Time.time;
        }

        private void Update()
        {
            float curTime = Time.time - _startTime;
            if (curTime > duration)
            {
                curTime -= duration;
                float t = curTime / missTime;
                t = Mathf.Clamp01(t);
                // float value = 1f - Mathf.Pow(t, 5f);
                float value = Mathf.Pow(1f - t, 5f);
                float yOffset = value * moveHeight / 2 + moveHeight / 2;
                transform.position = beforePos + new Vector3(0, yOffset, 0);
                _damageText.alpha = value;
            }
            else
            {
                float t = curTime / duration;
                t = Mathf.Clamp01(t);
                float value = 1f - Mathf.Pow(1f - t, 5f);
                float yOffset = value * moveHeight;
                transform.position = beforePos + new Vector3(0, yOffset, 0);
                transform.localScale = Vector3.one * value;
            }
            if (Time.time - _startTime > duration + missTime)
            {
                Destroy(gameObject);
            }
        }
    }
}