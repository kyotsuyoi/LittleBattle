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
        private readonly float defaultSpeed;
        private readonly bool movementX;
        private readonly float alpha;

        private float _rotate = 0;

        public Layer(Texture2D texture, float depth, float moveScale, bool movementX = false, float defaultSpeed = 1f, float alpha = 1f)
        {
            _texture = texture;
            _depth = depth;
            _moveScale = moveScale;
            this.defaultSpeed = defaultSpeed;
            this.alpha = alpha;
            Vector2 _position = new Vector2(Globals.Size.Width/2, Globals.Size.Height - texture.Height);
            positions = new List<Vector2>
            {
                _position
            };

            this.movementX = movementX;
        }

        public void Update()
        {
            Vector2 _position = positions[0];
            Globals.GroundX = _position.X;
            _position.X += (Globals.CameraMovement * _moveScale);
            if (movementX) _position.X += Globals.ElapsedSeconds * defaultSpeed;
            positions = new List<Vector2>
            {
                _position
            };

            float countX = _position.X;
            while (countX > 0)
            {
                _position.X -= _texture.Width;
                positions.Add(_position);
                countX -= _texture.Width;
            }

            int i = 0;
            int resRight = (int)((Globals.Size.Width - _position.X) / _texture.Width);
            _position = positions[0];
            while (i < resRight)
            {
                _position.X += _texture.Width;
                positions.Add(_position);
                i++;
            };
        }

        public void Draw()
        {
            foreach(var position in positions)
            {
                Globals.SpriteBatch.Draw(_texture, position, null, Color.White * alpha, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, _depth);
            };
        }
    }
}
