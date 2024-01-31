using LittleBattle.Classes;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LittleBattle.Manager
{
    public class DebugManager
    {
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Sprite player1, Sprite player2, List<SpriteBot> bots) {

            spriteBatch.DrawString(font, "Time:" + Globals.TotalSeconds, new Vector2(10, 0), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Time:" + Globals.TotalSeconds, new Vector2(12, 2), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Position.X:" + player1.Position.X, new Vector2(10, 20), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Position.X:" + player1.Position.X, new Vector2(12, 22), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Position.Y:" + player1.Position.Y, new Vector2(10, 40), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Position.Y:" + player1.Position.Y, new Vector2(12, 42), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "GroundX:" + Globals.GroundX.ToString(), new Vector2(10, 60), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "GroundX:" + Globals.GroundX.ToString(), new Vector2(12, 62), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.RelativeX:" + player1.RelativeX.ToString(), new Vector2(10, 80), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.RelativeX:" + player1.RelativeX.ToString(), new Vector2(12, 82), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.GroundLevel:" + player1.GroundLevel.ToString(), new Vector2(10, 100), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.GroundLevel:" + player1.GroundLevel.ToString(), new Vector2(12, 102), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.HoldUp:" + player1.HoldUp, new Vector2(10, 120), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.HoldUp:" + player1.HoldUp, new Vector2(12, 122), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            spriteBatch.DrawString(font, "player1.HoldDown:" + player1.HoldDown, new Vector2(10, 140), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.HoldDown:" + player1.HoldDown, new Vector2(12, 142), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Jump:" + player1.Jump, new Vector2(10, 160), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Jump:" + player1.Jump, new Vector2(12, 162), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Climb:" + player1.Climb, new Vector2(10, 180), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Climb:" + player1.Climb, new Vector2(12, 182), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Attack:" + player1.Attack, new Vector2(10, 200), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Attack:" + player1.Attack, new Vector2(12, 202), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Globals.SpriteFrame:" + Globals.SpriteFrame.ToString(), new Vector2(10, 220), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Globals.SpriteFrame:" + Globals.SpriteFrame.ToString(), new Vector2(12, 222), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Attributes.AttackCooldown:" + player1.Attribute.AttackCooldown.ToString(), new Vector2(10, 240), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Attributes.AttackCooldown:" + player1.Attribute.AttackCooldown.ToString(), new Vector2(12, 242), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            spriteBatch.DrawString(font, "player1.Combo:" + player1.GetCombo(), new Vector2(10, 260), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Combo:" + player1.GetCombo(), new Vector2(12, 262), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            spriteBatch.DrawString(font, "player1.ComboTimeLimit:" + player1.Attribute.ComboTimeLimit, new Vector2(10, 280), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.ComboTimeLimit:" + player1.Attribute.ComboTimeLimit, new Vector2(12, 282), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            var count = player1.GetSpriteFXCount();
            foreach (var bot in bots)
            {
                count += bot.GetSpriteFXCount();
            }
            spriteBatch.DrawString(font, "FXCount:" + count.ToString(), new Vector2(10, 300), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "FXCount:" + count.ToString(), new Vector2(12, 302), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "player1.Ground:" + player1.Ground, new Vector2(10, 320), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "player1.Ground:" + player1.Ground, new Vector2(12, 322), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Wood:" + player1.GetBagItem(Enums.SpriteType.Wood), new Vector2(10, 340), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Wood:" + player1.GetBagItem(Enums.SpriteType.Wood), new Vector2(12, 342), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Seed:" + player1.GetBagItem(Enums.SpriteType.Seed), new Vector2(10, 360), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Seed:" + player1.GetBagItem(Enums.SpriteType.Seed), new Vector2(12, 362), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Stone:" + player1.GetBagItem(Enums.SpriteType.Stone), new Vector2(10, 380), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Stone:" + player1.GetBagItem(Enums.SpriteType.Stone), new Vector2(12, 382), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            spriteBatch.DrawString(font, "Iron:" + player1.GetBagItem(Enums.SpriteType.Iron), new Vector2(10, 400), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Iron:" + player1.GetBagItem(Enums.SpriteType.Iron), new Vector2(12, 402), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);


        }

    }
}
