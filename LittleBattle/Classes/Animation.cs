using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LittleBattle.Classes
{
    public class Animation
    {
        private readonly Texture2D _texture;
        private readonly List<Rectangle> _sourceRectangles = new List<Rectangle>();
        private readonly int _frames;
        private int _frame;
        private readonly float _frameTime;
        private float _frameTimeLeft;
        private bool _active = true;
        private SpriteEffects spriteEffects;
        private bool loop;

        public bool EndLoop { get; set; }

        public Animation(Texture2D texture, int framesX, int framesY, int framesStart, int framesEnd, float frameTime, int row, bool flip, bool loop)
        {
            _texture = texture;
            _frameTime = frameTime;
            _frameTimeLeft = _frameTime;
            _frames = framesX;
            this.loop = loop;
            EndLoop = false;

            var frameWidth = _texture.Width / framesX;
            var frameHeight = _texture.Height / framesY;

            spriteEffects = SpriteEffects.None;
            if (flip)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            int framesCount = 0;
            for (int i = 0; i < _frames; i++)
            {
                if (i >= framesStart && i <= framesEnd)
                {
                    _sourceRectangles.Add(new Rectangle(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
                    framesCount++;
                }
            }
            _frames = framesCount;
        }

        public void Stop()
        {
            _active = false;
        }

        public void Start()
        {
            _active = true;
        }

        public void Reset()
        {
            _frame = 0;
            _frameTimeLeft = _frameTime;
            EndLoop = false;
        }

        public void Update(bool fastAnimation, int specificFrame = -1)
        {
            if(!loop)Globals.SpriteFrame = _frame;

            if (!_active) return;
            if (specificFrame != -1)
            {
                _frame = specificFrame;
                return;
            }

            _frameTimeLeft -= Globals.ElapsedSeconds;
            if (fastAnimation)
            {
                _frameTimeLeft -= Globals.ElapsedSeconds;
            }

            if (_frameTimeLeft <= 0)
            {
                _frameTimeLeft += _frameTime;
                if (!loop && _frame == _frames-1)
                {
                    EndLoop = true;
                    return;
                }
                _frame = (_frame + 1) % _frames;
            }
        }

        public void Draw(Vector2 pos, float layerdepth)
        {
            Globals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, Vector2.One, spriteEffects, layerdepth);
        }

        public Rectangle instantRect()
        {
            return _sourceRectangles[_frame];
        }
    }
}
