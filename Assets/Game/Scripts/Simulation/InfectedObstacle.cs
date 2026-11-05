namespace Game.Simulation
{
    public readonly struct InfectedObstacle
    {
        public InfectedObstacle(int index, int generation, float distance)
        {
            Index = index;
            Generation = generation;
            Distance = distance;
        }

        public int Index { get; }
        public int Generation { get; }
        public float Distance { get; }
    }
}
