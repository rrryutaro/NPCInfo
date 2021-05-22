using System;
using Microsoft.Xna.Framework;

namespace NPCInfo
{
    public static class VectorUtils
    {
        public static float ToDistance(this Vector2 v1, Vector2 v2)
        {
            float result = Vector2.Distance(v1, v2);
            return result;
        }

        public static float GetRadian(this Vector2 v1, Vector2 v2)
        {
            float result = (float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
            return result;
        }

        public static Vector2 ToRotationVector(this Vector2 v1, float distance, float angle, bool bRadian = false)
        {
            Vector2 result;
            double radian = bRadian ? angle : angle.ToRadian();
            result = v1 + new Vector2((float)Math.Cos(radian) * distance, (float)Math.Sin(radian) * distance);
            return result;
        }

        public static float ToRadian(this float f)
        {
            float result = (float)(f * Math.PI / 180);
            return result;
        }
    }
}
