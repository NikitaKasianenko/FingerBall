using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Configs;
using Game.Effects;
using Game.Level;
using Game.Simulation;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Game.Obstacles
{
    public class ObstacleField : MonoBehaviour, IResettable
    {
        private const int BatchSize = 1023;
        private const float BoundsSize = 500f;

        private static readonly Matrix4x4 PivotOffset = Matrix4x4.Translate(Vector3.up);

        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _normalMaterial;
        [SerializeField] private Material _infectedMaterial;
        [SerializeField] private ParticleBurst _explosion;
        [SerializeField] private ShadowCastingMode _shadows = ShadowCastingMode.On;

        [Header("Variation")]
        [SerializeField, Range(0f, 0.5f)] private float _heightVariation = 0.15f;
        [SerializeField, Range(0f, 20f)] private float _maxTilt = 7f;

        private LevelSession _session;
        private LevelConfig _level;
        private ExplosionConfig _config;
        private ObstacleInstance[] _instances = Array.Empty<ObstacleInstance>();
        private Matrix4x4[] _normalMatrices = Array.Empty<Matrix4x4>();
        private Matrix4x4[] _infectedMatrices = Array.Empty<Matrix4x4>();
        private RenderParams _normalParams;
        private RenderParams _infectedParams;

        public event Action Impacted;
        public event Action ObstacleExploded;

        [Inject]
        public void Construct(LevelSession session, LevelConfig level, ExplosionConfig config)
        {
            _session = session;
            _level = level;
            _config = config;
        }

        private void Awake()
        {
            _normalParams = CreateRenderParams(_normalMaterial);
            _infectedParams = CreateRenderParams(_infectedMaterial);
        }

        public void ResetState()
        {
            ObstacleSet obstacles = _session.Course.Obstacles;
            _instances = new ObstacleInstance[obstacles.Count];
            _normalMatrices = new Matrix4x4[obstacles.Count];
            _infectedMatrices = new Matrix4x4[obstacles.Count];
            _explosion.Clear();

            var random = new System.Random(_session.Index);
            float diameter = _level.ObstacleRadius * 2f;
            for (int i = 0; i < obstacles.Count; i++)
            {
                Vector2 point = obstacles.PositionOf(i);
                float height = _level.ObstacleHeight * (1f + _heightVariation * NextSigned(random));
                Quaternion rotation = Quaternion.Euler(_maxTilt * NextSigned(random), (float)random.NextDouble() * 360f,
                    _maxTilt * NextSigned(random));
                _instances[i] = new ObstacleInstance(new Vector3(point.x, 0f, point.y), rotation,
                    new Vector3(diameter, height * 0.5f, diameter));
            }
        }

        public async UniTask ExplodeAsync(IReadOnlyList<InfectedObstacle> infected, Vector3 impactPoint,
            CancellationToken cancellationToken)
        {
            _explosion.Burst(impactPoint, _config.ParticlesPerImpact);
            Impacted?.Invoke();

            float longestFuse = 0f;
            foreach (InfectedObstacle obstacle in infected)
            {
                float fuse = _config.FuseTime + obstacle.Generation * _config.ChainDelay
                                              + obstacle.Distance * _config.DistanceDelay;
                longestFuse = Mathf.Max(longestFuse, fuse);
                _instances[obstacle.Index].Infect(fuse);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(longestFuse + _config.SettleTime),
                cancellationToken: cancellationToken);
        }

        private void Update()
        {
            for (int i = 0; i < _instances.Length; i++)
            {
                if (_instances[i].Tick(Time.deltaTime))
                    Explode(_instances[i]);
            }
        }

        private void LateUpdate()
        {
            int normalCount = 0;
            int infectedCount = 0;
            for (int i = 0; i < _instances.Length; i++)
            {
                ObstacleInstance instance = _instances[i];
                if (instance.State == ObstacleState.Normal)
                    _normalMatrices[normalCount++] = MatrixOf(instance);
                else if (instance.State == ObstacleState.Infected)
                    _infectedMatrices[infectedCount++] = MatrixOf(instance);
            }

            Draw(_normalParams, _normalMatrices, normalCount);
            Draw(_infectedParams, _infectedMatrices, infectedCount);
        }

        private void Explode(ObstacleInstance instance)
        {
            _explosion.Burst(instance.Center, _config.ParticlesPerObstacle);
            ObstacleExploded?.Invoke();
        }

        private Matrix4x4 MatrixOf(ObstacleInstance instance) =>
            Matrix4x4.TRS(instance.Position, instance.Rotation, instance.ScaleWithPulse(_config.PulseScale)) * PivotOffset;

        private void Draw(RenderParams renderParams, Matrix4x4[] matrices, int count)
        {
            for (int start = 0; start < count; start += BatchSize)
                Graphics.RenderMeshInstanced(renderParams, _mesh, 0, matrices, Mathf.Min(BatchSize, count - start), start);
        }

        private RenderParams CreateRenderParams(Material material) =>
            new(material)
            {
                shadowCastingMode = _shadows,
                receiveShadows = true,
                worldBounds = new Bounds(Vector3.zero, Vector3.one * BoundsSize)
            };

        private static float NextSigned(System.Random random) =>
            (float)(random.NextDouble() * 2.0 - 1.0);
    }
}
