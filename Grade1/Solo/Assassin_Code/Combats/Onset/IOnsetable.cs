using UnityEngine;

namespace Code.Combats.Onset
{
    public interface IOnsetable
    {
        /// <summary>
        /// 플레이어를 감지했다면 true, 아니면 false
        /// </summary>
        bool IsFindPlayer { get; }
    }
}

