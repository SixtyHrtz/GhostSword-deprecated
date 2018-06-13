using System;

namespace GhostSword
{
    public class Random
    {
        private static System.Random random = new System.Random((int)DateTime.Now.Ticks);

        public static float Percent() => (float)random.NextDouble() * 100;
        public static uint UnsignedInteger(uint min, uint max) => (uint)random.Next((int)min, (int)max + 1);
    }
}
