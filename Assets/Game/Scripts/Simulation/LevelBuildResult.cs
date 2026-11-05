namespace Game.Simulation
{
    public readonly struct LevelBuildResult
    {
        public LevelBuildResult(ObstacleSet obstacles, int seed, SolveResult cheapest, SolveResult tapSpam,
            float usableVolume)
        {
            Obstacles = obstacles;
            Seed = seed;
            Cheapest = cheapest;
            TapSpam = tapSpam;
            UsableVolume = usableVolume;
        }

        public ObstacleSet Obstacles { get; }
        public int Seed { get; }
        public SolveResult Cheapest { get; }
        public SolveResult TapSpam { get; }
        public float UsableVolume { get; }
        public float BudgetUse => UsableVolume > 0f ? Cheapest.VolumeUsed / UsableVolume : 0f;
    }
}
