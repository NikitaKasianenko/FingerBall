using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class HouseConfig
    {
        [field: Header("Door")]
        [field: SerializeField, Min(0f)] public float OpenDistance { get; private set; } = 5f;
        [field: SerializeField, Min(0.05f)] public float OpenDuration { get; private set; } = 0.45f;
        [field: SerializeField] public float OpenAngle { get; private set; } = 105f;
        [field: SerializeField, Min(0.05f)] public float CloseDuration { get; private set; } = 0.25f;

        [field: Header("Celebration")]
        [field: SerializeField, Range(0f, 0.5f)] public float Squash { get; private set; } = 0.14f;
        [field: SerializeField, Min(0.05f)] public float SquashDuration { get; private set; } = 0.5f;
        [field: SerializeField, Min(0f)] public float CelebrationDuration { get; private set; } = 1.3f;
    }
}
