using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "DeceleratingRushPattern", menuName = "SO/Pattern/DeceleratingRushPattern", order = 0)]
    public class DeceleratingRushPatternSO : RushPatternSO
    {
        [Header("Deceleration")]
        [SerializeField, Min(0f)] private float startSpeedMultiplier = 1.9f;
        [SerializeField, Min(0f)] private float endSpeedMultiplier = 0.1f;

        protected override float GetDashSpeedMultiplier(float normalizedTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(normalizedTime));
            return Mathf.Lerp(startSpeedMultiplier, endSpeedMultiplier, t);
        }
    }
}
