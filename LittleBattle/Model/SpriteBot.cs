using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleBattle.Model
{
    public class SpriteBot : Sprite
    {
        public bool Patrol = false;
        public float PatrolX;
        public float PatrolX_Area;
        public float PatrolWait;
        public bool GoTo = false;

        public SpriteBot(int ID, Vector2 position, Enums.SpriteType spriteType, Enums.Team team, Enums.ClassType classType) : base(ID, position, spriteType, team, classType)
        {
        }

        public void SetInitialPatrolArea(float positionX)
        {
            PatrolX_Area = positionX;
            Patrol = true;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics)
        {
            _anims.Draw(Position, 0.9999f, deadAlpha);

            foreach (var attack in spriteFXs)
            {
                attack.Draw(spriteBatch, font, graphics, 0.1f);
            }

            if (ID == 01 || ID == 02) spriteBatch.DrawString(font, "*", new Vector2(Position.X + 18, Position.Y - 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);

            if (IsDead() || ID == 0) return;
            string mark = "";
            if (Patrol)
            {
                mark += "P ";
            }
            if (GoTo)
            {
                mark += "G ";
            }

            spriteBatch.DrawString(font, mark, new Vector2(Position.X + 18, Position.Y - 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 12, Position.Y-2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            Texture2D _texture;
            if (Globals.Debug && Globals.DebugArea)
            {
                _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
                _texture.SetData(new Color[] { Color.Blue });
                spriteBatch.Draw(_texture, GetRectangle(), Color.Blue * 0.4f);
            }

            var _hp_percent = Attribute.HP * 100 / Attribute.BaseHP;
            if (_hp_percent >= 100) return;
            var hp_val = Size.X * _hp_percent / 100;

            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X, 4), Color.Black * 0.6f);

            var _color = Color.GreenYellow;
            if (_hp_percent < 75) _color = Color.Yellow;
            if (_hp_percent < 50) _color = Color.Orange;
            if (_hp_percent < 25) _color = Color.Red;

            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { _color });
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)hp_val, 4), _color * 0.6f);

        }

    }
}
