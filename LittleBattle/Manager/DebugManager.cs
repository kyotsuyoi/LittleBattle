using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LittleBattle.Manager
{
    public class DebugManager
    {
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Sprite player1, Sprite player2, List<Sprite> bots) {

            spriteBatch.DrawString(font, "Time:" + Globals.TotalSeconds, new Vector2(10, 0), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Time:" + Globals.TotalSeconds, new Vector2(12, 2), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Position.X:" + player1.Position.X, new Vector2(10, 20), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Position.X:" + player1.Position.X, new Vector2(12, 22), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player2.Position.X:" + player2.Position.X, new Vector2(10, 40), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player2.Position.X:" + player2.Position.X, new Vector2(12, 42), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "GroundX:" + Globals.GroundX.ToString(), new Vector2(10, 60), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "GroundX:" + Globals.GroundX.ToString(), new Vector2(12, 62), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.RelativeX:" + (player1.RelativeX).ToString(), new Vector2(10, 80), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.RelativeX:" + (player1.RelativeX).ToString(), new Vector2(12, 82), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player2.RelativeX:" + (player2.RelativeX).ToString(), new Vector2(10, 100), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player2.RelativeX:" + (player2.RelativeX).ToString(), new Vector2(12, 102), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            //spriteBatch.DrawString(font, "bot1.RelativeX:" + (bots[0].RelativeX).ToString(), new Vector2(10, 120), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, "bot1.RelativeX:" + (bots[0].RelativeX).ToString(), new Vector2(12, 122), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            //spriteBatch.DrawString(font, "bot2.RelativeX:" + (bots[1].RelativeX).ToString(), new Vector2(10, 140), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, "bot2.RelativeX:" + (bots[1].RelativeX).ToString(), new Vector2(12, 142), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Jump:" + player1.Jump, new Vector2(10, 160), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Jump:" + player1.Jump, new Vector2(12, 162), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player2.Jump:" + player2.Jump, new Vector2(10, 180), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player2.Jump:" + player2.Jump, new Vector2(12, 182), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Attack:" + player1.Attack, new Vector2(10, 200), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Attack:" + player1.Attack, new Vector2(12, 202), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Globals.SpriteFrame:" + Globals.SpriteFrame.ToString(), new Vector2(10, 220), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Globals.SpriteFrame:" + Globals.SpriteFrame.ToString(), new Vector2(12, 222), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Attributes.AttackCooldown:" + player1.Attribute.AttackCooldown.ToString(), new Vector2(10, 240), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Attributes.AttackCooldown:" + player1.Attribute.AttackCooldown.ToString(), new Vector2(12, 242), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.GetSpriteFXCount:" + player1.GetSpriteFXCount().ToString(), new Vector2(10, 260), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.GetSpriteFXCount:" + player1.GetSpriteFXCount().ToString(), new Vector2(12, 262), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
        }

    }
}
