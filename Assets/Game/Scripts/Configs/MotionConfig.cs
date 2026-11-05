using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class MotionConfig
    {
        [field: Header("Hop")]
        [field: SerializeField, Min(0.1f)] public float HopLength { get; private set; } = 1.3f;
        [field: SerializeField, Min(0f)] public float HopHeight { get; private set; } = 0.55f;
        [field: SerializeField, Min(0.05f)] public float HopDuration { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 0.5f)] public float HopStretch { get; private set; } = 0.12f;

        [field: Header("Idle")]
        [field: SerializeField, Min(0f)] public float IdleBounceHeight { get; private set; } = 0.06f;
        [field: SerializeField, Min(0f)] public float IdleBounceFrequency { get; private set; } = 2.4f;

        [field: Header("Entering The House")]
        [field: SerializeField, Min(0.05f)] public float EnterDuration { get; private set; } = 0.7f;
        [field: SerializeField, Min(0f)] public float EnterDepth { get; private set; } = 1.6f;
        [field: SerializeField, Range(0f, 1f)] public float EnterEndScale { get; private set; } = 0.6f;

        [field: Header("Defeat")]
        [field: SerializeField, Range(0f, 0.5f), Tooltip("How much the ball wobbles while it deflates")]
        public float DefeatWobble { get; private set; } = 0.18f;
        [field: SerializeField, Min(0.05f)] public float DefeatWobbleDuration { get; private set; } = 0.6f;
        [field: SerializeField, Min(0.05f)] public float DefeatShrinkDuration { get; private set; } = 0.25f;
        [field: SerializeField, Min(0f), Tooltip("Pause after the ball pops before the result screen")]
        public float DefeatPause { get; private set; } = 0.35f;
    }
}
