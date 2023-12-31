using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

public class SpriteFX
{
    protected Texture2D texture;
    public Vector2 Position { get; set; }
    public Vector2 Size { get; }
    public float Speed { get; set; }
    public bool Walk { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public Vector2 Direction { get; set; }
    private Enums.Side Side { get; set; }
    public Enums.SpriteType spriteType { get; }
    public float RelativeX { get; set; }
    public bool Active { get; set; }

    private readonly AnimationManager _anims = new AnimationManager();
    public Sprite Owner { get; set; }
    public AttributeFX AttributeFX { get; set; }

    private List<int> DamagedIDs;

    public SpriteFX(Sprite Owner, Vector2 direction, Enums.SpriteType spriteType, Texture2D texture, int framesX, int framesY)
    {
        this.Owner = Owner;
        this.spriteType = spriteType;
        Active = true;

        AttributeFX = new AttributeFX();

        Speed = 1;

        Ground = false;
        FallingSpeed = 0;
        Direction = direction;

        this.texture = texture;
        _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));
        _anims.AddAnimation(Enums.Direction.WalkRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.WalkLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        GroundLevel = Globals.Size.Height - Size.Y -30;

        //Debug
        //if (Owner.ID == 1)
        //{
        //    AttributeFX.Range = 50;
        //}
        InitalPosition();
        DamagedIDs = new List<int>();        
    }

    public void Update()
    {
        AnimationResolve();
        //FallingResolve();

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

        var animRight = _anims.GetAnimation(Enums.Direction.StandRight);
        var animLeft = _anims.GetAnimation(Enums.Direction.StandLeft);

        if (animRight.EndLoop)
        {            
            Direction = Enums.Direction.None;
            Active = false;
        }

        if (animLeft.EndLoop)
        {
            Direction = Enums.Direction.None;
            Active = false;
        }
    }

    private void FallingResolve()
    {
        if (!Ground && FallingSpeed <= Globals.Gravity)
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
            Ground = true;
        }
        else
        {
            Ground = false;
        }
    }

    private void InitalPosition()
    {
        var oCenterX = Owner.Position.X + (Owner.Size.X / 2);
        var oCenterY = Owner.Position.Y + (Owner.Size.Y / 2);
        var centerX = Size.X / 2;
        var centerY = Size.Y / 2;

        Position = new Vector2(oCenterX - centerX, (oCenterY - centerY));
        if (Owner.GetSide() == Enums.Side.Right) Position += new Vector2(AttributeFX.Range, 0);
        if (Owner.GetSide() == Enums.Side.Left) Position -= new Vector2(AttributeFX.Range, 0);
    }

    public void Damage(Sprite target)
    {
        Collision collision = new Collision();
        var collide = collision.SquareCollision(Position, Size, target.Position, target.Size);
        if (collide && !DamagedIDs.Any(id=> id == target.ID))
        {
            target.TakeDamage(this);
            DamagedIDs.Add(target.ID);
        }
    }

    public float DirectionSpeed()
    {
        if (Direction == Enums.Direction.WalkLeft && Walk) return Speed;
        if (Direction == Enums.Direction.WalkRight && Walk) return -Speed;
        return 0;
    }

    public Enums.Side GetSide()
    {
        if (Direction == Enums.Direction.StandRight
            || Direction == Enums.Direction.WalkRight
            || Direction == Enums.Direction.AttackRight
            || Direction == Enums.Direction.DeadRight
        )
        {
            return Enums.Side.Right;
        }

        if (Direction == Enums.Direction.StandLeft
            || Direction == Enums.Direction.WalkLeft
            || Direction == Enums.Direction.AttackLeft
            || Direction == Enums.Direction.DeadLeft
        )
        {
            return Enums.Side.Left;
        }

        return Enums.Side.None;
    }

    public void Draw()
    {
        _anims.Draw(Position);
    }
}
