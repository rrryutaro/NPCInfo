using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace NPCInfo
{
    public static class DrawUtils
    {
        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, int width, Color color)
        {
            float width2 = width / 2f;
            float rotation = start.GetRadian(end);
            Vector2 scale = new Vector2(start.ToDistance(end), width2);
            Vector2 start2 = start.ToRotationVector(width2, rotation - (90f).ToRadian(), true);
            sb.DrawLine(start2, rotation, scale, width2, color);
            sb.DrawLine(start, rotation, scale, width2, color);
        }
        public static void DrawLine(this SpriteBatch sb, Vector2 start, float rotation, Vector2 scale, float width, Color color)
        {
            sb.Draw(Main.magicPixel, start - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
