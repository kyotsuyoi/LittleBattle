using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static LittleBattle.Classes.Enums;

namespace LittleBattle.Model
{
    public class SpriteCameraman : Sprite
    {

        public SpriteCameraman(int ID, Vector2 position, Enums.SpriteType spriteType, Enums.Team team, Enums.ClassType classType) : base(ID, position, spriteType, team, classType)
        {
        }

        protected override void SetTexture()
        {
            int framesX = 4;
            int framesY = 4;

            texture = Globals.Content.Load<Texture2D>("Sprites/SpriteCameraman_x3");            

            _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 3, 0.25f, 1, false, true));
            _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 3, 0.25f, 1, true, true));
            _anims.AddAnimation(Enums.Direction.WalkRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 2, false, true));
            _anims.AddAnimation(Enums.Direction.WalkLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 2, true, true));
            _anims.AddAnimation(Enums.Direction.AttackRight, new Animation(texture, framesX, framesY, 0, 3, 0.05f, 3, false, false));
            _anims.AddAnimation(Enums.Direction.AttackLeft, new Animation(texture, framesX, framesY, 0, 3, 0.05f, 3, true, false));
            _anims.AddAnimation(Enums.Direction.DeadRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 4, false, false));
            _anims.AddAnimation(Enums.Direction.DeadLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 4, true, false));
            _anims.AddAnimation(Enums.Direction.StuntRight, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 5, false, false));
            _anims.AddAnimation(Enums.Direction.StuntLeft, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 5, true, false));
            _anims.AddAnimation(Enums.Direction.ClimbUp, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 6, false, true));
            _anims.AddAnimation(Enums.Direction.ClimbDown, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 6, false, true));

            Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        }

        public override void Update()
        {
            SetAnimationResolve();
            AttackResolve();
            UpdateCooldown();
            JumpResolve();
            FallingResolve();

            RelativeX = Position.X - Globals.GroundX;

            if (Combo && Attribute.ComboTimeLimit == 0)
            {
                Combo = false;
            }

            _anims.Update(Direction, Walk);
        }

        protected override void SetAnimationResolve()
        {
            if (IsDead())
            {
                if (spriteType == SpriteType.Bot)
                {
                    deadAlpha -= 0.1f * Globals.ElapsedSeconds;
                    if (deadAlpha <= 0) Active = false;
                }
                return;
            }

            AnimationResolve(0);
        }

        public override void SetMovement(bool move, Side side)
        {
            if (IsDead() || Attribute.StuntTime > 0)
            {
                Walk = false;
                return;
            }

            var PositiveLimit = Globals.PositiveLimit.Width - Globals.Size.Width / 2;
            var NegativeLimit = Globals.NegativeLimit.Width + Globals.Size.Width / 2;

            if (Climb)
            {
                if (side == Side.Up && Position.Y > 0)
                {
                    Direction = Enums.Direction.ClimbUp;
                }
                else if (side == Side.Down && Position.Y + Size.Y < GroundLevel)
                {
                    Direction = Enums.Direction.ClimbDown;
                }
                else
                {
                    Direction = Enums.Direction.None;
                }
                return;
            }

            if (move)
            {
                if (side == Side.Right && RelativeX + Size.X / 2 < PositiveLimit)
                {
                    Direction = Enums.Direction.WalkRight;
                    Walk = true;
                }
                else if (side == Side.Left && RelativeX > NegativeLimit)
                {
                    Direction = Enums.Direction.WalkLeft;
                    Walk = true;
                }
                else
                {
                    Walk = false;
                }
            }
            else
            {
                if (side == Side.Right)
                {
                    Direction = Enums.Direction.StandRight;
                }
                else if (side == Side.Left)
                {
                    Direction = Enums.Direction.StandLeft;
                }
                else if (side == Side.Up)
                {
                    HoldUp = true;
                    HoldDown = false;
                }
                else if (side == Side.Down)
                {
                    HoldUp = false;
                    HoldDown = true;
                }
                else
                {
                    HoldUp = false;
                    HoldDown = false;
                }
                Walk = false;
            }
        }

    }
}
