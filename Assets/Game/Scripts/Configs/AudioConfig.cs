using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class AudioConfig
    {
        [field: Header("Charge")]
        [field: SerializeField] public AudioClip ChargeLoop { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float ChargeVolume { get; private set; } = 0.35f;
        [field: SerializeField, Min(0.1f)] public float ChargePitchMin { get; private set; } = 0.8f;
        [field: SerializeField, Min(0.1f)] public float ChargePitchMax { get; private set; } = 2.2f;

        [field: Header("Shot")]
        [field: SerializeField] public SoundEffect Shoot { get; private set; }
        [field: SerializeField] public SoundEffect Impact { get; private set; }
        [field: SerializeField] public SoundEffect ObstaclePop { get; private set; }
        [field: SerializeField, Min(0f), Tooltip("Pops closer together than this are skipped so chains do not clip")]
        public float MinPopInterval { get; private set; } = 0.035f;

        [field: Header("Ball And House")]
        [field: SerializeField] public SoundEffect Hop { get; private set; }
        [field: SerializeField] public SoundEffect Deflate { get; private set; }
        [field: SerializeField] public SoundEffect BallPop { get; private set; }
        [field: SerializeField] public SoundEffect DoorOpen { get; private set; }

        [field: Header("Result And UI")]
        [field: SerializeField] public SoundEffect Win { get; private set; }
        [field: SerializeField] public SoundEffect Lose { get; private set; }
        [field: SerializeField] public SoundEffect Click { get; private set; }
    }
}
