using UnityEngine;

namespace Game.Obstacles
{
    public struct ObstacleInstance
    {
        private float _timer;
        private float _fuse;

        public ObstacleInstance(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            State = ObstacleState.Normal;
            _timer = 0f;
            _fuse = 0f;
        }

        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }
        public ObstacleState State { get; private set; }
        public Vector3 Center => Position + Rotation * Vector3.up * Scale.y;

        public void Infect(float fuse)
        {
            State = ObstacleState.Infected;
            _timer = 0f;
            _fuse = fuse;
        }

        public bool Tick(float deltaTime)
        {
            if (State != ObstacleState.Infected)
                return false;

            _timer += deltaTime;
            if (_timer < _fuse)
                return false;

            State = ObstacleState.Destroyed;
            return true;
        }

        public Vector3 ScaleWithPulse(float pulseScale)
        {
            if (State != ObstacleState.Infected || _fuse <= 0f)
                return Scale;

            float progress = Mathf.Clamp01(_timer / _fuse);
            return Scale * Mathf.Lerp(1f, pulseScale, progress * progress);
        }
    }
}
