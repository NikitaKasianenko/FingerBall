using System;
using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public class InfectionResolver
    {
        private bool[] _visited = Array.Empty<bool>();

        public void Resolve(ObstacleSet obstacles, int hitIndex, Vector2 center, float radius, InfectionConfig config,
            List<InfectedObstacle> result)
        {
            result.Clear();
            PrepareVisited(obstacles.Count);

            float directReach = radius + obstacles.Radius;
            float chainLimit = radius * config.ChainReach + obstacles.Radius;
            float chainSquared = config.ChainDistance * config.ChainDistance;

            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!obstacles.IsAlive(i))
                    continue;

                float distance = (obstacles.PositionOf(i) - center).magnitude;
                if (i == hitIndex || distance <= directReach)
                    Add(i, 0, distance, result);
            }

            for (int next = 0; next < result.Count; next++)
            {
                InfectedObstacle source = result[next];
                Vector2 sourcePosition = obstacles.PositionOf(source.Index);
                for (int j = 0; j < obstacles.Count; j++)
                {
                    if (_visited[j] || !obstacles.IsAlive(j))
                        continue;

                    Vector2 position = obstacles.PositionOf(j);
                    float distance = (position - center).magnitude;
                    if (distance <= chainLimit && (position - sourcePosition).sqrMagnitude <= chainSquared)
                        Add(j, source.Generation + 1, distance, result);
                }
            }
        }

        private void PrepareVisited(int count)
        {
            if (_visited.Length < count)
                _visited = new bool[count];
            else
                Array.Clear(_visited, 0, count);
        }

        private void Add(int index, int generation, float distance, List<InfectedObstacle> result)
        {
            _visited[index] = true;
            result.Add(new InfectedObstacle(index, generation, distance));
        }
    }
}
