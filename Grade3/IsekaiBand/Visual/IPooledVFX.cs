using UnityEngine;

namespace _Work.CHUH.Code.Visual
{
    public interface IPooledVFX
    {
        void Play(Vector3 position, float scale, Vector2 direction);
    }
}
