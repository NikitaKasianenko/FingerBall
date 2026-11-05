using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class CameraConfig
    {
        [field: SerializeField, Min(0f), Tooltip("The gameplay camera stops following this far before the door so the house stays in view")]
        public float StopBeforeEnd { get; private set; } = 12f;
        [field: SerializeField, Min(0f), Tooltip("Seconds to wait for the blend to the finish camera before the ball rolls in")]
        public float FinishBlendTime { get; private set; } = 0.8f;
        [field: SerializeField, Min(0f), Tooltip("Seconds to wait for the blend back before the result screen")]
        public float ReturnBlendTime { get; private set; } = 0.9f;
    }
}
