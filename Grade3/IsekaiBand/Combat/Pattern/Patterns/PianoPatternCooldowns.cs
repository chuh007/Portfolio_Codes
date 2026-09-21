using System.Collections.Generic;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoPatternCooldowns
    {
        private readonly Dictionary<object, float> _cooldowns = new();
        private static readonly List<object> s_CooldownKeys = new();

        public bool IsPatternOnCooldown(object key)
        {
            return key != null && _cooldowns.TryGetValue(key, out float remaining) && remaining > 0f;
        }

        public void StartCooldown(object key, float duration)
        {
            if (key == null || duration <= 0f)
                return;

            _cooldowns[key] = duration;
        }

        public void UpdateCooldowns()
        {
            if (_cooldowns.Count == 0)
                return;

            s_CooldownKeys.Clear();
            s_CooldownKeys.AddRange(_cooldowns.Keys);
            for (int i = 0; i < s_CooldownKeys.Count; i++)
            {
                object key = s_CooldownKeys[i];
                _cooldowns[key] = Mathf.Max(0f, _cooldowns[key] - Time.deltaTime);
            }
        }
    }
}
