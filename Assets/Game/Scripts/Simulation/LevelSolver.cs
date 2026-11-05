using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public class LevelSolver
    {
        private const float SpendableMargin = 0.999f;

        private readonly BallConfig _ball;
        private readonly ShotConfig _shot;
        private readonly InfectionConfig _infection;
        private readonly BalanceConfig _balance;
        private readonly List<InfectedObstacle> _infected = new();

        public LevelSolver(BallConfig ball, ShotConfig shot, InfectionConfig infection, BalanceConfig balance)
        {
            _ball = ball;
            _shot = shot;
            _infection = infection;
            _balance = balance;
        }

        public SolveResult SolveCheapest(ObstacleSet source, PathAxis axis)
        {
            SolveResult best = SolveGreedy(source, axis);
            for (int k = 0; k < _balance.SolverCandidates; k++)
            {
                float volume = CandidateVolume(_shot.MinVolume, _ball.UsableVolume, k);
                SolveResult result = SolveFixed(source, axis, volume);
                if (result.Success && (!best.Success || result.VolumeUsed < best.VolumeUsed))
                    best = result;
            }

            return best;
        }

        public SolveResult SolveFixed(ObstacleSet source, PathAxis axis, float shotVolume)
        {
            Course course = CreateCourse(source, axis);
            float volume = _ball.StartVolume;
            float distance = 0f;
            int shots = 0;
            while (shots <= _balance.SolverMaxShots)
            {
                distance = course.NextStop(distance, SizeModel.RadiusFromVolume(volume), out bool reachesEnd);
                if (reachesEnd)
                    return new SolveResult(true, _ball.StartVolume - volume, shots);

                if (volume - shotVolume <= _ball.CriticalVolume)
                    break;

                Shoot(course, distance, volume, shotVolume);
                volume -= shotVolume;
                shots++;
            }

            return new SolveResult(false, _ball.StartVolume - volume, shots);
        }

        private SolveResult SolveGreedy(ObstacleSet source, PathAxis axis)
        {
            Course course = CreateCourse(source, axis);
            Course scratch = CreateCourse(source, axis);
            float volume = _ball.StartVolume;
            float distance = 0f;
            int shots = 0;
            while (shots <= _balance.SolverMaxShots)
            {
                distance = course.NextStop(distance, SizeModel.RadiusFromVolume(volume), out bool reachesEnd);
                if (reachesEnd)
                    return new SolveResult(true, _ball.StartVolume - volume, shots);

                float spendable = (volume - _ball.CriticalVolume) * SpendableMargin;
                if (spendable <= _shot.MinVolume)
                    break;

                float bestVolume = PickGreedyVolume(course, scratch, distance, volume, spendable);
                Shoot(course, distance, volume, bestVolume);
                volume -= bestVolume;
                shots++;
            }

            return new SolveResult(false, _ball.StartVolume - volume, shots);
        }

        private float PickGreedyVolume(Course course, Course scratch, float distance, float volume, float spendable)
        {
            float bestScore = float.MinValue;
            float bestVolume = _shot.MinVolume;
            for (int k = 0; k < _balance.SolverCandidates; k++)
            {
                float candidate = CandidateVolume(_shot.MinVolume, spendable, k);
                scratch.Obstacles.CopyStateFrom(course.Obstacles);
                float score = Shoot(scratch, distance, volume, candidate) / candidate;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestVolume = candidate;
                }
            }

            return bestVolume;
        }

        private float Shoot(Course course, float distance, float volume, float shotVolume)
        {
            float ballRadius = SizeModel.RadiusFromVolume(volume - shotVolume);
            course.Infect(course.PlanShot(distance, ballRadius, shotVolume), _infected);
            return course.NextStop(distance, ballRadius, out _) - distance;
        }

        private Course CreateCourse(ObstacleSet source, PathAxis axis) =>
            new(axis, source.Clone(), _infection, _balance.StopGap);

        private float CandidateVolume(float min, float max, int index) =>
            min * Mathf.Pow(Mathf.Max(1f, max / min), index / (float)(_balance.SolverCandidates - 1));
    }
}
