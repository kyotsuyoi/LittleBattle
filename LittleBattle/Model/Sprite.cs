using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite
{
    protected Texture2D texture;
    public Vector2 Position { get; set; }
    public Vector2 Size { get; }
    public float Speed { get; set; }
    public bool Walk { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Attack { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public float JumpPower { get; set; }
    public Vector2 Direction { get; set; }
    public Enums.SpriteType spriteType { get; }
    public float RelativeX { get; set; }

    private readonly AnimationManager _anims = new AnimationManager();

    public Sprite(Vector2 position, Enums.SpriteType spriteType, Texture2D texture, int framesX, int framesY)
    {
        Position = position;
        this.spriteType = spriteType;

        Speed = 1;
        if (spriteType == Enums.SpriteType.Bot) Speed = 0.5f;

        Jump = false;
        Ground = false;
        FallingSpeed = 0;
        JumpPower = 5;
        Direction = Enums.Direction.StandRight;

        this.texture = texture;
        _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 3, 0.25f, 1, false, true));
        _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 3, 0.25f, 1, true, true));
        _anims.AddAnimation(Enums.Direction.WalkRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 2, false, true));
        _anims.AddAnimation(Enums.Direction.WalkLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 2, true, true));
        _anims.AddAnimation(Enums.Direction.AttackRight, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 3, false, false));
        _anims.AddAnimation(Enums.Direction.AttackLeft, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 3, true, false));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        GroundLevel = Globals.Size.Height - Size.Y -30;
    }

    public void Update()
    {
        AnimationResolve();
        JumpResolve();
        FallingResolve();
        AttackResolve();

        if (spriteType != Enums.SpriteType.Player1)
        {
            Position += new Vector2(Globals.CameraMovement,0);
        }
        RelativeX = Position.X - Globals.GroundX;

        _anims.Update(Direction, Walk);
    }

    private void AnimationResolve()
    {
        var speed = Speed;
        if (spriteType != Enums.SpriteType.Player1) speed = Speed * 2;
        if ((Position.X <= 0 && Direction == Enums.Direction.WalkLeft)
            || (Position.X >= Globals.Size.Width - Size.X && Direction == Enums.Direction.WalkRight)) Walk = false;

        if (Walk)
        {
            if (Direction == Enums.Direction.WalkLeft)
            {
                Position += new Vector2(-speed, 0);
            }
            else if (Direction == Enums.Direction.WalkRight)
            {
                Position += new Vector2(speed, 0);
            }
        }
        else
        {
            if (Direction == Enums.Direction.WalkLeft || Direction == Enums.Direction.StandLeft)
            {
                Direction = Enums.Direction.StandLeft;
            }
            else if (Direction == Enums.Direction.WalkRight || Direction == Enums.Direction.StandRight)
            {
                Direction = Enums.Direction.StandRight;
            }
        }
    }

    private void FallingResolve()
    {
        if (!Ground && !Jump && FallingSpeed <= Globals.Gravity)
        {
            FallingSpeed += 0.25f;
            Position += new Vector2(0, FallingSpeed);
            if (FallingSpeed >= Globals.Gravity) FallingSpeed = Globals.Gravity;
        }
        else
        {
            FallingSpeed = 0;
        }

        if (Position.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel);
            JumpPower = 5;
            Ground = true;
        }
        else
        {
            Ground = false;
        }
    }

    private void JumpResolve()
    {
        if (Jump)
        {
            Ground = false;
            Position += new Vector2(0, -JumpPower);
            JumpPower -= 0.25f;
            if (JumpPower <= 0)
            {
                JumpPower = 0;
                Jump = false;
            }
        }
    }

    private void AttackResolve()
    {
        if (Attack)
        {
            if (Direction == Enums.Direction.WalkRight || Direction == Enums.Direction.StandRight)
            {
                Direction = Enums.Direction.AttackRight;
            }
            if (Direction == Enums.Direction.WalkLeft || Direction == Enums.Direction.StandLeft)
            {
                Direction = Enums.Direction.AttackLeft;
            }
            //Attack = false;
        }
        var animRight = _anims.GetAnimation(Enums.Direction.AttackRight);
        var animLeft = _anims.GetAnimation(Enums.Direction.AttackLeft);

        if (animRight.EndLoop)
        {
            animRight.Reset();
            Attack = false;
            Direction = Enums.Direction.StandRight;
        }

        if (animLeft.EndLoop)
        {
            animLeft.Reset();
            Attack = false;
            Direction = Enums.Direction.StandLeft;
        }

    }

    public float DirectionSpeed()
    {
        if (Direction == Enums.Direction.WalkLeft && Walk) return Speed;
        if (Direction == Enums.Direction.WalkRight && Walk) return -Speed;
        return 0;
    }

    public void Draw()
    {
        _anims.Draw(Position);
    }
}
