using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public static class LevelGenerator
    {
        private const float HexRowFactor = 0.866f;
        private const int SinglePlacementTries = 20;

        public static List<Vector2> GeneratePath(LevelConfig config, int seed)
        {
            var result = new List<Vector2>();
            AddPathContent(config, config.CreateAxis(), new System.Random(seed), result);
            return result;
        }

        public static List<Vector2> GenerateForest(LevelConfig config, int seed)
        {
            var result = new List<Vector2>();
            AddForest(config, config.CreateAxis(), seed, new System.Random(seed), result);
            return result;
        }

        private static void AddForest(LevelConfig config, PathAxis axis, int seed, System.Random random,
            List<Vector2> result)
        {
            var noise = new ValueNoise(seed);
            int columns = Mathf.FloorToInt((config.ForestMax.x - config.ForestMin.x) / config.ForestCellSize) + 1;
            int rows = Mathf.FloorToInt((config.ForestMax.y - config.ForestMin.y) / config.ForestCellSize) + 1;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var cell = new Vector2(config.ForestMin.x + column * config.ForestCellSize,
                        config.ForestMin.y + row * config.ForestCellSize);
                    Vector2 point = cell + new Vector2(NextSigned(random), NextSigned(random)) * config.ForestJitter;
                    float roll = (float)random.NextDouble();

                    float n = noise.Sample(cell.x * config.ForestNoiseScale, cell.y * config.ForestNoiseScale);
                    float dense = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(
                        config.ForestDenseThreshold - config.ForestBlend, config.ForestDenseThreshold + config.ForestBlend, n));
                    bool isBehindDoor = axis.Along(point) > axis.Length;
                    float probability = isBehindDoor
                        ? config.BackdropProbability
                        : Mathf.Lerp(config.ForestSparseProbability, config.ForestDenseProbability, dense);
                    bool isOnPath = !isBehindDoor && Mathf.Abs(axis.Across(point)) < config.PathBandHalfWidth;
                    if (roll < probability && !isOnPath && IsFree(config, axis, point))
                        result.Add(point);
                }
            }
        }

        private static void AddPathContent(LevelConfig config, PathAxis axis, System.Random random,
            List<Vector2> result)
        {
            float from = config.StartClearRadius + config.ContentMargin;
            float to = axis.Length - config.DoorClearRadius - config.ContentMargin;
            float segment = (to - from) / Mathf.Max(1, config.ClusterCount);

            var clusters = new List<(Vector2 Center, float Radius)>();
            for (int c = 0; c < config.ClusterCount; c++)
            {
                float radius = Mathf.Lerp(config.ClusterRadiusMin, config.ClusterRadiusMax, (float)random.NextDouble());
                float along = from + segment * (c + 0.5f) + NextSigned(random) * Mathf.Max(0f, segment * 0.5f - radius);
                Vector2 center = axis.PointAt(along) + axis.Normal * (NextSigned(random) * config.ClusterAcrossOffset);
                clusters.Add((center, radius));
                FillCluster(config, axis, center, radius, random, result);
            }

            var singles = new List<Vector2>();
            int tries = config.SingleCount * SinglePlacementTries;
            for (int attempt = 0; attempt < tries && singles.Count < config.SingleCount; attempt++)
            {
                float along = Mathf.Lerp(from, to, (float)random.NextDouble());
                Vector2 point = axis.PointAt(along) + axis.Normal * (NextSigned(random) * config.SingleAcrossOffset);
                if (IsSeparated(point, clusters, singles, config.SingleSeparation))
                    singles.Add(point);
            }

            result.AddRange(singles);
        }

        private static void FillCluster(LevelConfig config, PathAxis axis, Vector2 center, float radius,
            System.Random random, List<Vector2> result)
        {
            float rowStep = config.ClusterSpacing * HexRowFactor;
            int rowCount = Mathf.FloorToInt(radius / rowStep);
            int columnCount = Mathf.FloorToInt(radius / config.ClusterSpacing) + 1;
            for (int row = -rowCount; row <= rowCount; row++)
            {
                float shift = (row & 1) == 0 ? 0f : config.ClusterSpacing * 0.5f;
                for (int column = -columnCount; column <= columnCount; column++)
                {
                    var local = new Vector2(column * config.ClusterSpacing + shift, row * rowStep);
                    if (local.magnitude > radius)
                        continue;

                    Vector2 jitter = new Vector2(NextSigned(random), NextSigned(random)) * config.ClusterJitter;
                    Vector2 point = center + axis.Direction * local.y + axis.Normal * local.x + jitter;
                    if (Mathf.Abs(axis.Across(point)) < config.PathBandHalfWidth && IsFree(config, axis, point))
                        result.Add(point);
                }
            }
        }

        private static bool IsSeparated(Vector2 point, List<(Vector2 Center, float Radius)> clusters,
            List<Vector2> singles, float separation)
        {
            foreach ((Vector2 center, float radius) in clusters)
            {
                if (Vector2.Distance(point, center) < radius + separation)
                    return false;
            }

            foreach (Vector2 single in singles)
            {
                if (Vector2.Distance(point, single) < separation)
                    return false;
            }

            return true;
        }

        private static bool IsFree(LevelConfig config, PathAxis axis, Vector2 point)
        {
            float behindDoor = axis.Along(point) - axis.Length;
            bool insideHouse = behindDoor >= 0f && behindDoor <= config.HouseDepth
                                               && Mathf.Abs(axis.Across(point)) <= config.HouseHalfWidth;
            return !insideHouse
                   && Vector2.Distance(point, axis.Start) >= config.StartClearRadius
                   && Vector2.Distance(point, axis.End) >= config.DoorClearRadius;
        }

        private static float NextSigned(System.Random random) =>
            (float)(random.NextDouble() * 2.0 - 1.0);
    }
}
