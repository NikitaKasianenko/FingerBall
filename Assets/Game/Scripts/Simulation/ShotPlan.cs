namespace Game.Simulation
{
    public readonly struct ShotPlan
    {
        public ShotPlan(bool hasTarget, int hitIndex, float impactDistance, float shotVolume)
        {
            HasTarget = hasTarget;
            HitIndex = hitIndex;
            ImpactDistance = impactDistance;
            ShotVolume = shotVolume;
        }

        public bool HasTarget { get; }
        public int HitIndex { get; }
        public float ImpactDistance { get; }
        public float ShotVolume { get; }
        public float ShotRadius => SizeModel.RadiusFromVolume(ShotVolume);
    }
}
