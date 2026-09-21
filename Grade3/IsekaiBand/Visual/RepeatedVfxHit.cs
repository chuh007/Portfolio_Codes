using System.Collections;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    internal class RepeatedVfxHit
    {
        private readonly MonoBehaviour _owner;
        private Coroutine _routine;

        public RepeatedVfxHit(MonoBehaviour owner) => _owner = owner;

        public int Start(Func<int> hit, float interval)
        {
            Stop();
            int firstHitCount = hit();
            if (interval > 0f && _owner.isActiveAndEnabled)
                _routine = _owner.StartCoroutine(Repeat(hit, interval));
            return firstHitCount;
        }

        public void Stop()
        {
            if (_routine == null) return;
            _owner.StopCoroutine(_routine);
            _routine = null;
        }

        private IEnumerator Repeat(Func<int> hit, float interval)
        {
            var wait = new WaitForSeconds(interval);
            while (true)
            {
                yield return wait;
                if (!_owner.isActiveAndEnabled) yield break;
                hit();
            }
        }
    }
}
