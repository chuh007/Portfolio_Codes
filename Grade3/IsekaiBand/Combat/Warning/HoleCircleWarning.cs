using UnityEngine;

namespace _Work.CHUH.Code.Combat.Warning
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HoleCircleWarning : BaseWarning
    {
        [SerializeField] private int _segments = 36;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        public void Setup(Vector2 position, float innerRadius, float outerRadius)
        {
            base.Setup(position, 0f);
            _meshFilter.mesh = BuildMesh(innerRadius, outerRadius);
        }

        private Mesh BuildMesh(float inner, float outer)
        {
            var vertices = new Vector3[_segments * 2];
            var triangles = new int[_segments * 6];

            float step = 2f * Mathf.PI / _segments;

            for (int i = 0; i < _segments; i++)
            {
                float rad = step * i;
                float x = Mathf.Cos(rad);
                float y = Mathf.Sin(rad);
                vertices[i] = new Vector3(x * outer, y * outer);
                vertices[i + _segments] = new Vector3(x * inner, y * inner);
            }

            for (int i = 0; i < _segments; i++)
            {
                int next = (i + 1) % _segments;
                int ti = i * 6;
                triangles[ti]     = i;
                triangles[ti + 1] = next;
                triangles[ti + 2] = i + _segments;
                triangles[ti + 3] = next;
                triangles[ti + 4] = next + _segments;
                triangles[ti + 5] = i + _segments;
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        public override void ResetItem()
        {
            if (_meshFilter != null)
                _meshFilter.mesh = null;
        }
    }
}