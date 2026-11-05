using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public class Course
    {
        private readonly InfectionConfig _infection;
        private readonly float _stopGap;
        private readonly InfectionResolver _resolver = new();

        public Course(PathAxis axis, ObstacleSet obstacles, InfectionConfig infection, float stopGap)
        {
            Axis = axis;
            Obstacles = obstacles;
            _infection = infection;
            _stopGap = stopGap;
        }

        public PathAxis Axis { get; }
        public ObstacleSet Obstacles { get; }

        public float NextStop(float distance, float ballRadius, out bool reachesEnd)
        {
            reachesEnd = !PathQuery.TryFindBlocker(Obstacles, Axis, distance, ballRadius, out _, out float contact);
            return reachesEnd ? Axis.Length : Mathf.Max(distance, contact - _stopGap);
        }

        public ShotPlan PlanShot(float distance, float ballRadius, float shotVolume)
        {
            float shotRadius = SizeModel.RadiusFromVolume(shotVolume);
            float launch = distance + ballRadius + shotRadius;
            bool hasTarget = PathQuery.TryFindImpact(Obstacles, Axis, distance, ballRadius, shotRadius, launch,
                out int hitIndex, out float impact);
            return new ShotPlan(hasTarget, hitIndex, hasTarget ? impact : Axis.Length, shotVolume);
        }

        public void Infect(ShotPlan plan, List<InfectedObstacle> infected)
        {
            infected.Clear();
            if (!plan.HasTarget)
                return;

            _resolver.Resolve(Obstacles, plan.HitIndex, Axis.PointAt(plan.ImpactDistance),
                _infection.RadiusFor(plan.ShotVolume), _infection, infected);
            foreach (InfectedObstacle obstacle in infected)
                Obstacles.Remove(obstacle.Index);
        }
    }
}
