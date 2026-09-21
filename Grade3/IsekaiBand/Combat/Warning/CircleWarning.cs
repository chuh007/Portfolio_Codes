using UnityEngine;

namespace _Work.CHUH.Code.Combat.Warning
{
    public class CircleWarning : BaseWarning
    {
        [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 0.45f);
        [SerializeField] private int fillSegments = 48;

        private Transform _fill;
        private MeshRenderer _fillRenderer;
        private MaterialPropertyBlock _fillPropertyBlock;

        private void Awake()
        {
            CreateFill();
        }

        public new void Setup(Vector2 position, float radius)
        {
            base.Setup(position, 0f);
            transform.localScale = Vector3.one * radius * 2f;
            SetFillProgress(0f);
        }

        public override void ResetItem()
        {
            base.ResetItem();
            transform.localScale = Vector3.one;
        }

        protected override void SetFillProgress(float progress)
        {
            CreateFill();

            float value = Mathf.Clamp01(progress);
            _fill.localPosition = Vector3.zero;
            _fill.localScale = Vector3.one * value;
        }

        private void CreateFill()
        {
            if (_fill != null) return;

            SpriteRenderer outline = GetComponent<SpriteRenderer>();
            var fillObject = new GameObject("Fill");
            _fill = fillObject.transform;
            _fill.SetParent(transform, false);

            MeshFilter filter = fillObject.AddComponent<MeshFilter>();
            _fillRenderer = fillObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = BuildCircleMesh();

            if (outline != null)
            {
                _fillRenderer.sharedMaterial = outline.sharedMaterial;
                _fillRenderer.sortingLayerID = outline.sortingLayerID;
                _fillRenderer.sortingOrder = outline.sortingOrder - 1;
            }

            _fillPropertyBlock = new MaterialPropertyBlock();
            _fillPropertyBlock.SetColor("_Color", fillColor);
            _fillPropertyBlock.SetColor("_BaseColor", fillColor);
            _fillRenderer.SetPropertyBlock(_fillPropertyBlock);
        }

        private Mesh BuildCircleMesh()
        {
            int segmentCount = Mathf.Max(3, fillSegments);
            var vertices = new Vector3[segmentCount + 1];
            var triangles = new int[segmentCount * 3];

            vertices[0] = Vector3.zero;
            float step = Mathf.PI * 2f / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                float rad = step * i;
                vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * 0.5f;
            }

            for (int i = 0; i < segmentCount; i++)
            {
                int next = i + 1 == segmentCount ? 1 : i + 2;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = next;
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
