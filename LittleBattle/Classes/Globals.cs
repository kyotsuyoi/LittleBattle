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
        public static Size Size { get; set;  }
        public static float ElapsedSeconds { get; set; }
        public static float Gravity { get; set; }
        public static void Update(GameTime gameTime)
        {
            ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
