using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class InfectionConfig
    {
        [field: SerializeField, Min(0f)] public float RadiusScale { get; private set; } = 32f;
        [field: SerializeField, Min(1f), Tooltip("Above 1 the radius grows faster than the spent volume, so one big shot beats many taps")]
        public float VolumeExponent { get; private set; } = 1.5f;
        [field: SerializeField, Min(0f)] public float MaxRadius { get; private set; } = 2.6f;

        [field: Header("Chain")]
        [field: SerializeField, Min(0f), Tooltip("Infection jumps between obstacles closer than this")]
        public float ChainDistance { get; private set; } = 0.85f;
        [field: SerializeField, Min(1f), Tooltip("Chain reach as a multiple of the shot radius")]
        public float ChainReach { get; private set; } = 1.3f;

        public float RadiusFor(float shotVolume) =>
            Mathf.Min(MaxRadius, RadiusScale * Mathf.Pow(shotVolume, VolumeExponent));
    }
}
