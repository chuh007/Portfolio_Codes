using UnityEngine;

namespace _Work.CHUH.Code.Combat.Warning
{
    public enum RectWarningVisualMode
    {
        SplitLine,
        FillRect,
        SingleLine
    }

    // rotation 0 = 위쪽(+Y) 방향 기준. Vector2.SignedAngle(Vector2.up, dir)로 계산해서 넘길 것
    public class RectWarning : BaseWarning
    {
        [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 0.45f);
        [SerializeField, Min(0f)] private float lineWidth = 0.06f;
        [SerializeField, Range(0f, 1f)] private float splitOffsetRatio = 0.25f;

        private const int LeftOuter = 0;
        private const int LeftInner = 1;
        private const int RightInner = 2;
        private const int RightOuter = 3;
        private const int CenterLine = 4;
        private const int LineSegmentCount = 5;

        private readonly LineRenderer[] _segments = new LineRenderer[LineSegmentCount];
        private SpriteRenderer _sourceRenderer;
        private Transform _fill;
        private MeshRenderer _fillRenderer;
        private MaterialPropertyBlock _fillPropertyBlock;
        private float _width = 1f;
        private float _length = 1f;
        private bool _growForward;
        private RectWarningVisualMode _visualMode = RectWarningVisualMode.SplitLine;

        private void Awake()
        {
            _sourceRenderer = GetComponent<SpriteRenderer>();
            CreateLineSegments();
            CreateFill();
            ApplyVisualMode();
        }

        public void Setup(
            Vector2 position,
            float rotation,
            float width,
            float length,
            bool growForward = false,
            RectWarningVisualMode visualMode = RectWarningVisualMode.SplitLine)
        {
            base.Setup(position, rotation);
            _width = Mathf.Max(0f, width);
            _length = Mathf.Max(0f, length);
            _growForward = growForward;
            _visualMode = visualMode;
            transform.localScale = _visualMode == RectWarningVisualMode.FillRect
                ? new Vector3(_width, _length, 1f)
                : Vector3.one;
            ApplyVisualMode();
            SetFillProgress(0f);
        }

        public override void ResetItem()
        {
            _width = 1f;
            _length = 1f;
            _growForward = false;
            _visualMode = RectWarningVisualMode.SplitLine;
            transform.localScale = Vector3.one;
            ApplyVisualMode();
            base.ResetItem();
        }

        protected override void SetFillProgress(float progress)
        {
            float value = Mathf.Clamp01(progress);
            if (_visualMode == RectWarningVisualMode.FillRect)
            {
                SetRectFillProgress(value);
                return;
            }

            if (_visualMode == RectWarningVisualMode.SingleLine)
            {
                SetSingleLineProgress(value);
                return;
            }

            SetSplitLineProgress(value);
        }

        private void SetSplitLineProgress(float progress)
        {
            CreateLineSegments();

            float halfWidth = _width * 0.5f;
            float halfLength = _length * 0.5f;
            float splitOffset = Mathf.Max(lineWidth * 2f, _width * splitOffsetRatio) * (1f - progress);

            SetSegment(LeftOuter, -halfWidth - splitOffset, -halfLength, halfLength);
            SetSegment(LeftInner, -halfWidth + splitOffset, -halfLength, halfLength);
            SetSegment(RightInner, halfWidth - splitOffset, -halfLength, halfLength);
            SetSegment(RightOuter, halfWidth + splitOffset, -halfLength, halfLength);
        }

        private void SetSingleLineProgress(float progress)
        {
            CreateLineSegments();

            float halfLength = _length * 0.5f;
            float fromY = _growForward ? -halfLength : Mathf.Lerp(0f, -halfLength, progress);
            float toY = _growForward ? Mathf.Lerp(-halfLength, halfLength, progress) : Mathf.Lerp(0f, halfLength, progress);
            SetSegment(CenterLine, 0f, fromY, toY);
        }

        private void SetRectFillProgress(float progress)
        {
            CreateFill();

            if (_growForward)
            {
                _fill.localPosition = new Vector3(0f, -0.5f + progress * 0.5f, 0f);
                _fill.localScale = new Vector3(1f, progress, 1f);
                return;
            }

            _fill.localPosition = Vector3.zero;
            _fill.localScale = new Vector3(progress, progress, 1f);
        }

        private void ApplyVisualMode()
        {
            bool useFillRect = _visualMode == RectWarningVisualMode.FillRect;
            bool useSingleLine = _visualMode == RectWarningVisualMode.SingleLine;

            if (_sourceRenderer != null)
                _sourceRenderer.enabled = useFillRect;

            if (_fill != null)
                _fill.gameObject.SetActive(useFillRect);

            for (int i = 0; i < LineSegmentCount; i++)
            {
                if (_segments[i] != null)
                    _segments[i].enabled = useSingleLine ? i == CenterLine : !useFillRect && i != CenterLine;
            }
        }

        private void CreateLineSegments()
        {
            if (_segments[0] != null) return;

            _sourceRenderer ??= GetComponent<SpriteRenderer>();

            for (int i = 0; i < LineSegmentCount; i++)
            {
                var lineObject = new GameObject($"LineSegment_{i}");
                lineObject.transform.SetParent(transform, false);

                LineRenderer segment = lineObject.AddComponent<LineRenderer>();
                segment.useWorldSpace = false;
                segment.positionCount = 2;
                segment.widthMultiplier = lineWidth;
                segment.numCapVertices = 0;
                segment.numCornerVertices = 0;
                segment.startColor = fillColor;
                segment.endColor = fillColor;

                if (_sourceRenderer != null)
                {
                    segment.sharedMaterial = _sourceRenderer.sharedMaterial;
                    segment.sortingLayerID = _sourceRenderer.sortingLayerID;
                    segment.sortingOrder = _sourceRenderer.sortingOrder;
                }

                _segments[i] = segment;
            }
        }

        private void CreateFill()
        {
            if (_fill != null) return;

            _sourceRenderer ??= GetComponent<SpriteRenderer>();
            var fillObject = new GameObject("Fill");
            _fill = fillObject.transform;
            _fill.SetParent(transform, false);

            MeshFilter filter = fillObject.AddComponent<MeshFilter>();
            _fillRenderer = fillObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = BuildQuadMesh();

            if (_sourceRenderer != null)
            {
                _fillRenderer.sharedMaterial = _sourceRenderer.sharedMaterial;
                _fillRenderer.sortingLayerID = _sourceRenderer.sortingLayerID;
                _fillRenderer.sortingOrder = _sourceRenderer.sortingOrder - 1;
            }

            _fillPropertyBlock = new MaterialPropertyBlock();
            _fillPropertyBlock.SetColor("_Color", fillColor);
            _fillPropertyBlock.SetColor("_BaseColor", fillColor);
            _fillRenderer.SetPropertyBlock(_fillPropertyBlock);
        }

        private void SetSegment(int index, float x, float fromY, float toY)
        {
            LineRenderer segment = _segments[index];
            segment.widthMultiplier = lineWidth;
            segment.startColor = fillColor;
            segment.endColor = fillColor;
            segment.SetPosition(0, new Vector3(x, fromY, 0f));
            segment.SetPosition(1, new Vector3(x, toY, 0f));
        }

        private Mesh BuildQuadMesh()
        {
            var mesh = new Mesh();
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f)
            };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
