using System;
using Game.Simulation;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class BallConfig
    {
        [field: SerializeField, Min(0.1f)] public float StartRadius { get; private set; } = 1f;
        [field: SerializeField, Min(0.05f), Tooltip("The ball loses when it shrinks to this radius")]
        public float CriticalRadius { get; private set; } = 0.32f;

        public float StartVolume => SizeModel.VolumeFromRadius(StartRadius);
        public float CriticalVolume => SizeModel.VolumeFromRadius(CriticalRadius);
        public float UsableVolume => StartVolume - CriticalVolume;
    }
}
