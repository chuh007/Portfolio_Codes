using System;
using System.Threading;
using Chuh007Lib.ObjectPool.RunTime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Warning
{
    public enum WarningType
    {
        Circle,
        CircularSector,
        HoleCircle,
        Square
    }
    
    // 경고 어케만들지
    // 범위 타입이 있을만한게 사각형, 원, 부채꼴에 속 빈 원 정도인가 Ray쏴서 하는거도 있긴한데 본질적으로 사각형과 비슷
    // 경고가 지속될 시간, 범위 어케깔지, 회전,
    // 패턴을 SO로 하지 말까 패턴 SO의 장점이 무엇인가?
    // SO가 재사용성에서는 상당히 좋긴 하니 SO 유지는 하자
    public abstract class BaseWarning : MonoBehaviour, IPoolable
    {
        public virtual void Setup(Vector2 position, float rotation = 0f)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }

        public virtual async UniTask PlayAsync(float duration, CancellationToken ct)
        {
            if (this == null)
                return;

            SetFillProgress(0f);

            if (duration > 0f)
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (this == null || ct.IsCancellationRequested) break;

                    SetFillProgress(Mathf.Clamp01(elapsed / duration));
                    elapsed += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                }
            }

            if (this != null && !ct.IsCancellationRequested)
                SetFillProgress(1f);

            TryReturnToPool();
        }

        protected virtual void SetFillProgress(float progress)
        {
        }

        
        #region Pool

        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        private Pool _myPool;
        
        public virtual void ResetItem()
        {
            SetFillProgress(0f);
        }

        protected void ReturnToPool() => TryReturnToPool();

        private void TryReturnToPool()
        {
            if (this == null || _myPool == null)
                return;

            _myPool.Push(this);
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        #endregion
    }
}
