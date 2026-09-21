using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoSpriteFactory
    {
        private readonly PianoBossRuntime _runtime;
        private Sprite _solidSprite;

        public PianoSpriteFactory(PianoBossRuntime runtime) => _runtime = runtime;

        public GameObject CreateSpriteObject(string objectName, Sprite sprite, Vector2 position, Color color, int sortingOrder)
        {
            Sprite safeSprite = sprite != null ? sprite : GetSolidSprite();
            if (safeSprite == null)
                return null;

            var obj = new GameObject(objectName);
            obj.transform.position = position;

            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = safeSprite;
            renderer.color = color;
            renderer.sortingLayerName = _runtime.SortingLayerName;
            renderer.sortingOrder = sortingOrder;

            return obj;
        }

        public void FitSpriteToRect(GameObject obj, Rect rect)
        {
            SpriteRenderer renderer = obj != null ? obj.GetComponent<SpriteRenderer>() : null;
            if (renderer == null || renderer.sprite == null)
                return;

            Vector2 spriteSize = renderer.sprite.bounds.size;
            if (spriteSize.x <= 0.001f || spriteSize.y <= 0.001f)
                return;

            obj.transform.localScale = new Vector3(rect.width / spriteSize.x, rect.height / spriteSize.y, 1f);
        }

        public Sprite GetSolidSprite()
        {
            if (_solidSprite != null)
                return _solidSprite;

            var texture = new Texture2D(1, 1)
            {
                hideFlags = HideFlags.DontSave,
                filterMode = FilterMode.Point
            };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            _solidSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            _solidSprite.hideFlags = HideFlags.DontSave;
            return _solidSprite;
        }
    }
}
