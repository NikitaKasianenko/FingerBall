using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class ShotConfig
    {
        [field: SerializeField, Min(0.001f), Tooltip("Volume taken from the ball by a single tap")]
        public float MinVolume { get; private set; } = 0.035f;
        [field: SerializeField, Min(0f)] public float ChargeRate { get; private set; } = 0.05f;
        [field: SerializeField, Min(0f), Tooltip("The charge rate grows by this much every second of holding")]
        public float ChargeAcceleration { get; private set; } = 0.16f;
        [field: SerializeField, Min(0.1f)] public float FlightSpeed { get; private set; } = 18f;

        public float ChargeRateAt(float holdTime) =>
            ChargeRate + ChargeAcceleration * holdTime;
    }
}
