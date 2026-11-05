using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class SoundEffect
    {
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 0.5f), Tooltip("Random pitch offset so repeated sounds do not sound identical")]
        public float PitchJitter { get; private set; }
    }
}
