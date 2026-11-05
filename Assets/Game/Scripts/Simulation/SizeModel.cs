using System;

namespace Game.Simulation
{
    public static class SizeModel
    {
        private const double OneThird = 1.0 / 3.0;

        public static float RadiusFromVolume(float volume) =>
            volume <= 0f ? 0f : (float)Math.Pow(volume, OneThird);

        public static float VolumeFromRadius(float radius) =>
            radius * radius * radius;
    }
}
