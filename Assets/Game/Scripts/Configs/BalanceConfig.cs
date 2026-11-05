using System;
using UnityEngine;

namespace Game.Configs
{
    [Serializable]
    public class BalanceConfig
    {
        [field: SerializeField] public int BaseSeed { get; private set; } = 1234;
        [field: SerializeField, Min(1)] public int SeedStride { get; private set; } = 7919;
        [field: SerializeField, Min(0f)] public float StopGap { get; private set; } = 0.05f;

        [field: Header("Level Check")]
        [field: SerializeField, Range(0f, 1f), Tooltip("The starting ball must hold this much more volume than the cheapest clear needs")]
        public float Reserve { get; private set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Layouts cheaper than this share of the usable volume are too easy")]
        public float MinBudgetUse { get; private set; } = 0.45f;
        [field: SerializeField, Min(1)] public int MaxAttempts { get; private set; } = 40;
        [field: SerializeField, Min(2)] public int SolverCandidates { get; private set; } = 12;
        [field: SerializeField, Min(1)] public int SolverMaxShots { get; private set; } = 120;

        public int SeedFor(int levelIndex) =>
            BaseSeed + levelIndex * SeedStride;
    }
}
