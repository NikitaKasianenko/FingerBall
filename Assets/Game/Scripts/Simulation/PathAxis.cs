using UnityEngine;

namespace Game.Simulation
{
    public readonly struct PathAxis
    {
        public PathAxis(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            Vector2 delta = end - start;
            Length = delta.magnitude;
            Direction = Length > 0f ? delta / Length : Vector2.up;
            Normal = new Vector2(Direction.y, -Direction.x);
        }

        public Vector2 Start { get; }
        public Vector2 End { get; }
        public Vector2 Direction { get; }
        public Vector2 Normal { get; }
        public float Length { get; }

        public Vector3 WorldDirection => new(Direction.x, 0f, Direction.y);
        public Vector3 WorldNormal => new(Normal.x, 0f, Normal.y);

        public Vector2 PointAt(float distance) =>
            Start + Direction * distance;

        public Vector3 ToWorld(float distance, float height)
        {
            Vector2 point = PointAt(distance);
            return new Vector3(point.x, height, point.y);
        }

        public float Along(Vector2 point) =>
            Vector2.Dot(point - Start, Direction);

        public float Across(Vector2 point) =>
            Vector2.Dot(point - Start, Normal);
    }
}
