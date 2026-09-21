using UnityEngine;

namespace _Work.CHUH.Code.Visual
{
    [DisallowMultipleComponent]
    public sealed class BossProjectileOutline : MonoBehaviour
    {
        private static Material _outlineMaterial;
        private SpriteRenderer _renderer;
        private Material _originalMaterial;
        private bool _isApplied;

        public static void ApplyTo(GameObject target)
        {
            if (target == null) return;

            foreach (SpriteRenderer renderer in target.GetComponentsInChildren<SpriteRenderer>(true))
                ApplyTo(renderer);
        }

        public static void ApplyTo(Renderer renderer)
        {
            if (renderer is not SpriteRenderer spriteRenderer || spriteRenderer == null) return;

            if (_outlineMaterial == null)
                _outlineMaterial = Resources.Load<Material>("EnemyProjectileOutlineRed");
            if (_outlineMaterial == null) return;

            var outline = spriteRenderer.GetComponent<BossProjectileOutline>();
            if (outline == null)
                outline = spriteRenderer.gameObject.AddComponent<BossProjectileOutline>();
            outline.Apply(spriteRenderer);
        }

        private void Apply(SpriteRenderer renderer)
        {
            if (!_isApplied)
            {
                _renderer = renderer;
                _originalMaterial = renderer.sharedMaterial;
            }

            renderer.sharedMaterial = _outlineMaterial;
            _isApplied = true;
        }

        private void OnDisable()
        {
            // 공용 투사체나 참격 이펙트가 풀에서 재사용될 때 원래 재질로 돌린다.
            if (_isApplied && _renderer != null)
                _renderer.sharedMaterial = _originalMaterial;
            _isApplied = false;
        }
    }
}
