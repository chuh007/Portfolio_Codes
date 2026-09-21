using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Combat.Warning
{
    // rotation 0 = 오른쪽(+X) 방향 기준. Vector2.SignedAngle(Vector2.right, dir)로 계산해서 넘길 것
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SectorWarning : BaseWarning
    {
        [SerializeField] private int segments = 24;
        [SerializeField] private string sortingLayerName = "GroundEffect";
        [SerializeField] private int sortingOrder = 1;
        [SerializeField] private Color outlineColor = new Color(1f, 0f, 0f, 0.75f);
        [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 0.45f);
        [SerializeField] private float outlineWidth = 0.05f;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshFilter _fillMeshFilter;
        private MeshRenderer _fillMeshRenderer;
        private LineRenderer _outlineRenderer;
        private MaterialPropertyBlock _fillPropertyBlock;
        private float _radius;
        private float _angle;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sortingLayerName = sortingLayerName;
            _meshRenderer.sortingOrder = sortingOrder;
            _meshRenderer.enabled = false;
            CreateFill();
            CreateOutline();
        }

        public void Setup(Vector2 position, float rotation, float radius, float angle)
        {
            base.Setup(position, rotation);
            _radius = radius;
            _angle = angle;
            _meshFilter.mesh = BuildMesh(radius, angle);
            BuildOutline(radius, angle);
            SetFillProgress(0f);
        }

        private Mesh BuildMesh(float radius, float angle)
        {
            int vertCount = segments + 2;
            var vertices = new Vector3[vertCount];
            var triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
            float step = angle * Mathf.Deg2Rad / segments;

            for (int i = 0; i <= segments; i++)
            {
                float rad = -halfAngle + step * i;
                vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            }

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        protected override void SetFillProgress(float progress)
        {
            CreateFill();
            FillMesh(_fillMeshFilter.mesh, _radius * Mathf.Clamp01(progress), _angle);
        }

        public async UniTask PlayTrackingAsync(float duration, Transform owner, Transform target, CancellationToken ct)
        {
            SetFillProgress(0f);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (ct.IsCancellationRequested) break;
                Vector2 dir = ((Vector2)(target.position - owner.position)).normalized;
                transform.position = owner.position;
                transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, dir));
                SetFillProgress(duration > 0f ? elapsed / duration : 1f);
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            }

            if (!ct.IsCancellationRequested)
                SetFillProgress(1f);

            ReturnToPool();
        }

        public override void ResetItem()
        {
            base.ResetItem();
            if (_meshFilter != null)
                _meshFilter.mesh = null;

            if (_fillMeshFilter != null)
                _fillMeshFilter.mesh.Clear();

            if (_outlineRenderer != null)
                _outlineRenderer.positionCount = 0;
        }

        private void CreateFill()
        {
            if (_fillMeshFilter != null) return;

            var fillObject = new GameObject("Fill");
            Transform fill = fillObject.transform;
            fill.SetParent(transform, false);

            _fillMeshFilter = fillObject.AddComponent<MeshFilter>();
            _fillMeshRenderer = fillObject.AddComponent<MeshRenderer>();
            _fillMeshFilter.sharedMesh = new Mesh();
            _fillMeshRenderer.sharedMaterial = _meshRenderer != null ? _meshRenderer.sharedMaterial : null;
            _fillMeshRenderer.sortingLayerName = sortingLayerName;
            _fillMeshRenderer.sortingOrder = sortingOrder;

            _fillPropertyBlock = new MaterialPropertyBlock();
            _fillPropertyBlock.SetColor("_Color", fillColor);
            _fillPropertyBlock.SetColor("_BaseColor", fillColor);
            _fillMeshRenderer.SetPropertyBlock(_fillPropertyBlock);
        }

        private void CreateOutline()
        {
            if (_outlineRenderer != null) return;

            var outlineObject = new GameObject("Outline");
            Transform outline = outlineObject.transform;
            outline.SetParent(transform, false);

            _outlineRenderer = outlineObject.AddComponent<LineRenderer>();
            _outlineRenderer.useWorldSpace = false;
            _outlineRenderer.loop = false;
            _outlineRenderer.widthMultiplier = outlineWidth;
            _outlineRenderer.sharedMaterial = _meshRenderer != null ? _meshRenderer.sharedMaterial : null;
            _outlineRenderer.startColor = outlineColor;
            _outlineRenderer.endColor = outlineColor;
            _outlineRenderer.sortingLayerName = sortingLayerName;
            _outlineRenderer.sortingOrder = sortingOrder + 1;
        }

        private void BuildOutline(float radius, float angle)
        {
            CreateOutline();

            int segmentCount = Mathf.Max(1, segments);
            _outlineRenderer.positionCount = segmentCount + 3;

            float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
            float step = angle * Mathf.Deg2Rad / segmentCount;

            _outlineRenderer.SetPosition(0, Vector3.zero);
            for (int i = 0; i <= segmentCount; i++)
            {
                float rad = -halfAngle + step * i;
                _outlineRenderer.SetPosition(i + 1, new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius);
            }
            _outlineRenderer.SetPosition(segmentCount + 2, Vector3.zero);
        }

        private void FillMesh(Mesh mesh, float radius, float angle)
        {
            int segmentCount = Mathf.Max(1, segments);
            int vertCount = segmentCount + 2;
            var vertices = new Vector3[vertCount];
            var triangles = new int[segmentCount * 3];

            vertices[0] = Vector3.zero;
            float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
            float step = angle * Mathf.Deg2Rad / segmentCount;

            for (int i = 0; i <= segmentCount; i++)
            {
                float rad = -halfAngle + step * i;
                vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            }

            for (int i = 0; i < segmentCount; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}
