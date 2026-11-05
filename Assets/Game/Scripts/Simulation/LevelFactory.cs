using System.Collections.Generic;
using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public class LevelFactory
    {
        private readonly BallConfig _ball;
        private readonly ShotConfig _shot;
        private readonly LevelConfig _level;
        private readonly BalanceConfig _balance;
        private readonly LevelSolver _solver;

        public LevelFactory(BallConfig ball, ShotConfig shot, InfectionConfig infection, LevelConfig level,
            BalanceConfig balance)
        {
            _ball = ball;
            _shot = shot;
            _level = level;
            _balance = balance;
            _solver = new LevelSolver(ball, shot, infection, balance);
        }

        public LevelBuildResult Build(int seed)
        {
            Candidate best = Evaluate(seed);
            for (int attempt = 1; attempt < _balance.MaxAttempts && !IsTarget(best); attempt++)
            {
                Candidate candidate = Evaluate(seed + attempt);
                if (Rank(candidate) > Rank(best))
                    best = candidate;
            }

            var obstacles = new List<Vector2>(best.Path);
            obstacles.AddRange(LevelGenerator.GenerateForest(_level, best.Seed));
            return new LevelBuildResult(new ObstacleSet(obstacles, _level.ObstacleRadius), best.Seed, best.Cheapest,
                best.TapSpam, _ball.UsableVolume);
        }

        private Candidate Evaluate(int seed)
        {
            List<Vector2> path = LevelGenerator.GeneratePath(_level, seed);
            var obstacles = new ObstacleSet(path, _level.ObstacleRadius);
            PathAxis axis = _level.CreateAxis();
            return new Candidate(seed, path, _solver.SolveCheapest(obstacles, axis),
                _solver.SolveFixed(obstacles, axis, _shot.MinVolume));
        }

        private bool IsValid(Candidate candidate) =>
            candidate.Cheapest.Success
            && candidate.Cheapest.VolumeUsed <= _ball.UsableVolume / (1f + _balance.Reserve)
            && !candidate.TapSpam.Success;

        private bool IsTarget(Candidate candidate) =>
            IsValid(candidate) && candidate.Cheapest.VolumeUsed >= _ball.UsableVolume * _balance.MinBudgetUse;

        private float Rank(Candidate candidate) =>
            IsValid(candidate) ? candidate.Cheapest.VolumeUsed : -1f;

        private readonly struct Candidate
        {
            public Candidate(int seed, List<Vector2> path, SolveResult cheapest, SolveResult tapSpam)
            {
                Seed = seed;
                Path = path;
                Cheapest = cheapest;
                TapSpam = tapSpam;
            }

            public int Seed { get; }
            public List<Vector2> Path { get; }
            public SolveResult Cheapest { get; }
            public SolveResult TapSpam { get; }
        }
    }
}
