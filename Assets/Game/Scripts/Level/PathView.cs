using Game.Ball;
using Game.Simulation;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    [RequireComponent(typeof(MeshFilter))]
    public class PathView : MonoBehaviour, IResettable
    {
        private const int EdgeCount = 4;
        private const float TraveledV = 0f;
        private const float AheadV = 1f;
        private const float BallV = 0.5f;
        private const float FadeV = 0.25f;

        [SerializeField, Min(0f)] private float _heightOffset = 0.02f;
        [SerializeField, Min(0.01f), Tooltip("Length of the colour fade between the traveled and the remaining path")]
        private float _fadeLength = 2f;

        private readonly Vector3[] _vertices = new Vector3[EdgeCount * 2];
        private readonly Vector2[] _uvs = new Vector2[EdgeCount * 2];
        private Mesh _mesh;
        private LevelSession _session;
        private BallView _ball;
        private PathAxis _axis;
        private float _startDistance;
        private bool _isReady;

        [Inject]
        public void Construct(LevelSession session, BallView ball)
        {
            _session = session;
            _ball = ball;
        }

        private void Awake()
        {
            _mesh = new Mesh { name = "Path" };
            _mesh.MarkDynamic();
            _mesh.vertices = _vertices;
            _mesh.uv = _uvs;

            var normals = new Vector3[_vertices.Length];
            var triangles = new int[(EdgeCount - 1) * 6];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = Vector3.up;

            for (int edge = 0; edge < EdgeCount - 1; edge++)
            {
                int vertex = edge * 2;
                int triangle = edge * 6;
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + 2;
                triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex + 2;
                triangles[triangle + 4] = vertex + 3;
                triangles[triangle + 5] = vertex + 1;
            }

            _mesh.normals = normals;
            _mesh.triangles = triangles;
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }

        private void OnDestroy() =>
            Destroy(_mesh);

        public void ResetState()
        {
            _axis = _session.Axis;
            _startDistance = -_session.Mass.BallRadius;
            _isReady = true;
        }

        private void LateUpdate()
        {
            if (!_isReady)
                return;

            float ball = _ball.Distance;
            float halfFade = _fadeLength * 0.5f;
            float fadeStart = Mathf.Max(_startDistance, ball - halfFade);
            float fadeEnd = Mathf.Min(_axis.Length, ball + halfFade);
            Vector3 side = _axis.WorldNormal * _ball.Radius;

            SetEdge(0, _startDistance, TraveledV, side);
            SetEdge(1, fadeStart, FadeVAt(fadeStart, ball, halfFade), side);
            SetEdge(2, fadeEnd, FadeVAt(fadeEnd, ball, halfFade), side);
            SetEdge(3, _axis.Length, AheadV, side);

            _mesh.vertices = _vertices;
            _mesh.uv = _uvs;
            _mesh.RecalculateBounds();
        }

        private static float FadeVAt(float distance, float ball, float halfFade) =>
            BallV + FadeV * (distance - ball) / halfFade;

        private void SetEdge(int edge, float distance, float v, Vector3 side)
        {
            Vector3 center = _axis.ToWorld(distance, _heightOffset);
            int vertex = edge * 2;
            _vertices[vertex] = transform.InverseTransformPoint(center - side);
            _vertices[vertex + 1] = transform.InverseTransformPoint(center + side);
            _uvs[vertex] = new Vector2(0f, v);
            _uvs[vertex + 1] = new Vector2(1f, v);
        }
    }
}
