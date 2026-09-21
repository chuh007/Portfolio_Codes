using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SlowFieldVisual
    {
        private readonly SlowFieldZone _zone;
        private Material _material;
        private Mesh _mesh;
        public SlowFieldVisual(SlowFieldZone zone) => _zone = zone;
        public void Destroy()
        {
            if (_mesh != null) UnityEngine.Object.Destroy(_mesh);
            if (_material != null) UnityEngine.Object.Destroy(_material);
        }

        public void CreateVisual(Color color, int segments, string sortingLayerName, int sortingOrder)
        {
            var visual = new GameObject("Visual");
            visual.transform.SetParent(_zone.transform, false);

            MeshFilter filter = visual.AddComponent<MeshFilter>();
            MeshRenderer renderer = visual.AddComponent<MeshRenderer>();

            _mesh = BuildCircleMesh(Mathf.Max(3, segments), _zone.Radius);
            filter.sharedMesh = _mesh;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Unlit");

            if (shader != null)
            {
                _material = new Material(shader);
                SetMaterialColor(color);
                renderer.sharedMaterial = _material;
            }

            renderer.sortingOrder = sortingOrder;
            if (!string.IsNullOrWhiteSpace(sortingLayerName))
                renderer.sortingLayerName = sortingLayerName;
        }

        public void UpdateVisualAlpha()
        {
            if (_material == null || _zone.Duration <= 0f)
                return;

            Color color = _material.color;
            color.a = Mathf.Lerp(color.a, 0f, Mathf.Clamp01(_zone.Elapsed / _zone.Duration) * Time.deltaTime);
            SetMaterialColor(color);
        }

        public void SetMaterialColor(Color color)
        {
            if (_material == null)
                return;

            _material.color = color;
            if (_material.HasProperty("_Color"))
                _material.SetColor("_Color", color);
            if (_material.HasProperty("_BaseColor"))
                _material.SetColor("_BaseColor", color);
        }

        public static Mesh BuildCircleMesh(int segments, float radius)
        {
            var vertices = new Vector3[segments + 1];
            var triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float step = Mathf.PI * 2f / segments;

            for (int i = 0; i < segments; i++)
            {
                float rad = step * i;
                vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
            }

            for (int i = 0; i < segments; i++)
            {
                int next = i + 1 == segments ? 1 : i + 2;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = next;
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
