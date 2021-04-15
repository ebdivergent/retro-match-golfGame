using UnityEngine;

namespace AppCore 
{
    public static class MathExtensions
    {
        public static float GetFractionalPart(this float val) 
        {
            return val - (int)val;
        }

        public static float Cycled(float value, float min, float max) 
        {
            float range = max - min;

            float t = GetFractionalPart((value - min) / range);

            return (range * t) + min;
        }
    }
}
