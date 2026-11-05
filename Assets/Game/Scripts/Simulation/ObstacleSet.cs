using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
    public class ObstacleSet
    {
        private readonly Vector2[] _positions;
        private readonly bool[] _alive;

        public ObstacleSet(IReadOnlyList<Vector2> positions, float radius)
        {
            _positions = new Vector2[positions.Count];
            _alive = new bool[positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                _positions[i] = positions[i];
                _alive[i] = true;
            }

            Radius = radius;
        }

        private ObstacleSet(Vector2[] positions, bool[] alive, float radius)
        {
            _positions = positions;
            _alive = alive;
            Radius = radius;
        }

        public int Count => _positions.Length;
        public float Radius { get; }

        public Vector2 PositionOf(int index) =>
            _positions[index];

        public bool IsAlive(int index) =>
            _alive[index];

        public void Remove(int index) =>
            _alive[index] = false;

        public ObstacleSet Clone() =>
            new(_positions, (bool[])_alive.Clone(), Radius);

        public void CopyStateFrom(ObstacleSet other)
        {
            if (other._positions != _positions)
                throw new ArgumentException("Obstacle sets must share the same layout.", nameof(other));

            Array.Copy(other._alive, _alive, _alive.Length);
        }
    }
}
