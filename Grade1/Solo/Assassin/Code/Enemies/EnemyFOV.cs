using Code.Entities;
using UnityEngine;

namespace Code.Enemies
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class EnemyFOV : MonoBehaviour, IEntityComponent
    {
        public float viewAngle = 0f;
        public float viewDistance = 0f;
        private int segmentCount = 10;

        public void Initialize(Entity entity)
        {

        }

        public void DrawFOV()
        {
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Vector3[] vertices = new Vector3[segmentCount + 2];
            int[] triangles = new int[segmentCount * 3];
            vertices[0] = Vector3.zero;
            float startAngle = -viewAngle / 2f;
            float angleStep = viewAngle / segmentCount;
            for (int i = 0; i <= segmentCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                float rad = Mathf.Deg2Rad * currentAngle;
                vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * viewDistance;
            }
            for (int i = 0; i < segmentCount; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}
