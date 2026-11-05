using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class ExplosionConfig
    {
        [field: SerializeField, Min(0f)] public float FuseTime { get; private set; } = 0.12f;
        [field: SerializeField, Min(0f), Tooltip("Extra delay for every chain step")]
        public float ChainDelay { get; private set; } = 0.14f;
        [field: SerializeField, Min(0f), Tooltip("Extra delay per metre from the impact point")]
        public float DistanceDelay { get; private set; } = 0.05f;
        [field: SerializeField, Min(1f)] public float PulseScale { get; private set; } = 1.25f;
        [field: SerializeField, Min(0f)] public float SettleTime { get; private set; } = 0.15f;

        [field: Header("Particles")]
        [field: SerializeField, Min(0)] public int ParticlesPerObstacle { get; private set; } = 10;
        [field: SerializeField, Min(0)] public int ParticlesPerImpact { get; private set; } = 18;
        [field: SerializeField, Min(0)] public int ParticlesPerLanding { get; private set; } = 8;
        [field: SerializeField, Min(0)] public int ParticlesPerBallPop { get; private set; } = 36;
    }
}
