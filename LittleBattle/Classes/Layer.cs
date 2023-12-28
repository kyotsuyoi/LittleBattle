using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LittleBattle.Classes
{
    public class Layer
    {
        private readonly Texture2D _texture;
        private List<Vector2> positions;
        private readonly float _depth;
        private readonly float _moveScale;
        private readonly float _defaultSpeed;
        private readonly bool _movementY;

        private float _rotate = 0;

        public Layer(Texture2D texture, float depth, float moveScale, bool movementY, float defaultSpeed = 0.0f)
        {
            _texture = texture;
            _depth = depth;
            _moveScale = moveScale;
            _defaultSpeed = defaultSpeed;
            Vector2 _position = new Vector2(0,Globals.Size.Height - texture.Height);
            positions = new List<Vector2>
            {
                _position
            };

            _movementY = movementY;
        }

        public void Update(float movement)
        {
            Vector2 _position = positions[0];
            _position.X += (movement * 0/*_moveScale*/);
            positions = new List<Vector2>
            {
                _position
            };

            int i = 0;
            int resRight = (int)((Globals.Size.Width - _position.X) / _texture.Width);
            int resLeft = (int)(_texture.Width / _position.X);
            while (i < resRight)
            {
                _position.X += _texture.Width;
                positions.Add(_position);
                i++;
            };
            i = -1;
            while (i < resLeft)
            {
                _position.X -= _texture.Width;
                positions.Add(_position);
                i++;
            };
        }

        public void Draw()
        {
            foreach(var position in positions)
            {
                Globals.SpriteBatch.Draw(_texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            };
        }
    }
}
