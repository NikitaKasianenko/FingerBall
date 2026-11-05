using System;
using Game.Configs;
using Game.Simulation;
using UnityEngine;

namespace Game.Level
{
    public class LevelSession
    {
        private readonly LevelFactory _factory;
        private readonly LevelConfig _level;
        private readonly BalanceConfig _balance;
        private readonly InfectionConfig _infection;

        public event Action Started;
        public event Action<LevelResult> Ended;

        public int Index { get; private set; }
        public int ShotsFired { get; private set; }
        public Course Course { get; private set; }
        public BallMass Mass { get; }
        public PathAxis Axis => Course.Axis;

        public LevelSession(BallConfig ball, ShotConfig shot, InfectionConfig infection, LevelConfig level,
            BalanceConfig balance)
        {
            _factory = new LevelFactory(ball, shot, infection, level, balance);
            _level = level;
            _balance = balance;
            _infection = infection;
            Mass = new BallMass(ball, shot);
        }

        public void Load(int index)
        {
            LevelBuildResult build = _factory.Build(_balance.SeedFor(index));
            Index = index;
            ShotsFired = 0;
            Course = new Course(_level.CreateAxis(), build.Obstacles, _infection, _balance.StopGap);
            Mass.Reset();

            Debug.Log($"Level {index + 1}: seed {build.Seed}, {build.Obstacles.Count} obstacles, " +
                      $"best play needs {build.BudgetUse:P0} of the ball in {build.Cheapest.ShotCount} shots, " +
                      $"tapping minimum shots fails after {build.TapSpam.ShotCount} shots");
        }

        public void Start() =>
            Started?.Invoke();

        public void RegisterShot() =>
            ShotsFired++;

        public void End(LevelResult result) =>
            Ended?.Invoke(result);
    }
}
