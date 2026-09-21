using System;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoPerformance
    {
        private readonly PianoBossRuntime _runtime;
        private float _performanceTimer;
        private int _performanceDepth;
        public bool IsActive => _performanceDepth > 0 || _performanceTimer > 0f;
        public PianoPerformance(PianoBossRuntime runtime) => _runtime = runtime;
        public void End() => _performanceDepth = Mathf.Max(0, _performanceDepth - 1);

        public IDisposable BeginPerformance()
        {
            _performanceDepth++;
            _runtime.PassiveBeat.StopPassivePattern();
            _runtime.PassiveBeat.ResetPassiveBeatTimer();
            return new PerformanceScope(_runtime);
        }

        public void PulsePerformance(float duration)
        {
            _performanceTimer = Mathf.Max(_performanceTimer, Mathf.Max(0f, duration));
            _runtime.PassiveBeat.StopPassivePattern();
            _runtime.PassiveBeat.ResetPassiveBeatTimer();
        }

        public void UpdatePerformancePulse()
        {
            if (_performanceTimer > 0f)
                _performanceTimer -= Time.deltaTime;
        }

        private sealed class PerformanceScope : IDisposable
        {
            private PianoBossRuntime _runtime;
            public PerformanceScope(PianoBossRuntime runtime) => _runtime = runtime;
            public void Dispose()
            {
                if (_runtime == null) return;
                _runtime.Performance.End();
                _runtime = null;
            }
        }
    }
}
