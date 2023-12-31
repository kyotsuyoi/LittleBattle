using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace LittleBattle.Classes
{
    public static class Globals
    {
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static Size Size { get; set; }
        public static Size PositiveLimit { get; set; }
        public static Size NegativeLimit { get; set; }
        public static float ElapsedSeconds { get; set; }
        public static float TotalSeconds { get; set; }
        public static float Gravity { get; set; }
        public static float GroundX { get; set; }
        public static float CameraMovement { get; set; }
        public static int SpriteFrame { get; set; }
        public static float GroundLevel { get; set; }

        private static int LastID = 10;
        public static bool Debug { get; set; }
        public static bool DebugArea { get; set; }

        public static void Update(GameTime gameTime)
        {
            ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
        }

        public static int GetNewID()
        {
            return LastID += 1;
        }

    }
}
