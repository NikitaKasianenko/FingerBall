namespace Game.Simulation
{
    public readonly struct SolveResult
    {
        public SolveResult(bool success, float volumeUsed, int shotCount)
        {
            Success = success;
            VolumeUsed = volumeUsed;
            ShotCount = shotCount;
        }

        public bool Success { get; }
        public float VolumeUsed { get; }
        public int ShotCount { get; }
    }
}
