using System;
using Game.Simulation;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class LevelConfig
    {
        [field: Header("Layout")]
        [field: SerializeField] public Vector2 Start { get; private set; } = new(-2.4f, 0f);
        [field: SerializeField] public Vector2 End { get; private set; } = new(2.4f, 26f);
        [field: SerializeField, Min(0.05f)] public float ObstacleRadius { get; private set; } = 0.33f;
        [field: SerializeField, Min(0.1f)] public float ObstacleHeight { get; private set; } = 1.5f;
        [field: SerializeField, Min(0f)] public float StartClearRadius { get; private set; } = 2.2f;
        [field: SerializeField, Min(0f)] public float DoorClearRadius { get; private set; } = 2.6f;
        [field: SerializeField, Min(0f), Tooltip("Half width of the house footprint kept free of obstacles")]
        public float HouseHalfWidth { get; private set; } = 2.5f;
        [field: SerializeField, Min(0f), Tooltip("Depth of the house footprint behind the door kept free of obstacles")]
        public float HouseDepth { get; private set; } = 3.6f;

        [field: Header("Obstacles On The Path")]
        [field: SerializeField, Min(0)] public int ClusterCount { get; private set; } = 4;
        [field: SerializeField, Min(0.3f)] public float ClusterRadiusMin { get; private set; } = 0.9f;
        [field: SerializeField, Min(0.3f)] public float ClusterRadiusMax { get; private set; } = 1.5f;
        [field: SerializeField, Min(0.3f)] public float ClusterSpacing { get; private set; } = 0.72f;
        [field: SerializeField, Min(0f)] public float ClusterJitter { get; private set; } = 0.1f;
        [field: SerializeField, Min(0f)] public float ClusterAcrossOffset { get; private set; } = 0.5f;
        [field: SerializeField, Min(0)] public int SingleCount { get; private set; } = 6;
        [field: SerializeField, Min(0f)] public float SingleAcrossOffset { get; private set; } = 0.8f;
        [field: SerializeField, Min(0f)] public float SingleSeparation { get; private set; } = 1.4f;
        [field: SerializeField, Min(0f)] public float ContentMargin { get; private set; } = 1f;

        [field: Header("Forest Beside The Path")]
        [field: SerializeField] public Vector2 ForestMin { get; private set; } = new(-12f, -6f);
        [field: SerializeField] public Vector2 ForestMax { get; private set; } = new(12f, 52f);
        [field: SerializeField, Min(0f), Tooltip("Keep wider than the starting ball radius plus obstacle radius so the forest never blocks the path")]
        public float PathBandHalfWidth { get; private set; } = 1.6f;
        [field: SerializeField, Min(0.3f)] public float ForestCellSize { get; private set; } = 0.95f;
        [field: SerializeField, Min(0f)] public float ForestJitter { get; private set; } = 0.2f;
        [field: SerializeField, Min(0.01f)] public float ForestNoiseScale { get; private set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f)] public float ForestSparseProbability { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 1f)] public float ForestDenseProbability { get; private set; } = 0.95f;
        [field: SerializeField, Range(0f, 1f)] public float ForestDenseThreshold { get; private set; } = 0.45f;
        [field: SerializeField, Range(0f, 0.5f)] public float ForestBlend { get; private set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Density of the forest behind the house, the backdrop of the finish")]
        public float BackdropProbability { get; private set; } = 0.9f;

        public PathAxis CreateAxis() =>
            new(Start, End);
    }
}
