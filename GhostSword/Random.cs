using System;

namespace GhostSword
{
    public class Random
    {
        private static System.Random random = new System.Random((int)DateTime.Now.Ticks);

        public static float Percent() => (float)random.NextDouble() * 100;
        public static int Integer(int min, int max) => random.Next(min, max + 1);
    }
}
