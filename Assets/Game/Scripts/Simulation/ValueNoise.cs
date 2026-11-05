using System;

namespace Game.Simulation
{
    public class ValueNoise
    {
        private const uint SeedMultiplier = 2654435761u;
        private const uint PrimeX = 374761393u;
        private const uint PrimeY = 668265263u;
        private const uint Mixer = 1274126177u;
        private const uint Mask = 0xFFFFFFu;

        private readonly uint _seed;

        public ValueNoise(int seed) =>
            _seed = unchecked((uint)seed * SeedMultiplier);

        public float Sample(float x, float y)
        {
            int x0 = (int)Math.Floor(x);
            int y0 = (int)Math.Floor(y);
            float u = Fade(x - x0);
            float v = Fade(y - y0);

            float bottom = Lerp(Hash(x0, y0), Hash(x0 + 1, y0), u);
            float top = Lerp(Hash(x0, y0 + 1), Hash(x0 + 1, y0 + 1), u);
            return Lerp(bottom, top, v);
        }

        private float Hash(int x, int y)
        {
            unchecked
            {
                uint h = _seed ^ ((uint)x * PrimeX) ^ ((uint)y * PrimeY);
                h = (h ^ (h >> 13)) * Mixer;
                h ^= h >> 16;
                return (h & Mask) / (float)Mask;
            }
        }

        private static float Fade(float t) =>
            t * t * (3f - 2f * t);

        private static float Lerp(float a, float b, float t) =>
            a + (b - a) * t;
    }
}
