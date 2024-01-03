using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static LittleBattle.Classes.Enums;

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

    private bool combo = false;

    public SpriteFX(Sprite Owner, Enums.Side side, Enums.SpriteType spriteType, int framesX, int framesY)
    {
        this.Owner = Owner;
        this.spriteType = spriteType;
        Active = true;

        AttributeFX = new AttributeFX(spriteType);

        Speed = 1;

        Ground = false;
        FallingSpeed = -1;
        Direction = Enums.Direction.StandRight;
        if (side == Side.Left) Direction = Enums.Direction.StandLeft;
        AttributeFX.Range += Owner.Attribute.Range;

        if (spriteType == SpriteType.SwordEffect) texture = Globals.Content.Load<Texture2D>("Sprites/SwordEffect");
        if (spriteType == SpriteType.ArrowEffect)
        {
            Walk = true;
            texture = Globals.Content.Load<Texture2D>("Sprites/ArrowEffect"); framesX = 1; framesY = 1;
        }

        _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));
        _anims.AddAnimation(Enums.Direction.WalkRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.WalkLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        GroundLevel = Globals.Size.Height - Size.Y -30;

        this.Position = Position;
        CenterX_Adjust();
        InitialPosition();
        DamagedIDs = new List<int>();        
    }

    public void Update()
    {
        AnimationResolve();
        FallingResolve();

        Position += new Vector2(Globals.CameraMovement,0);
        RelativeX = Position.X - Globals.GroundX;

        _anims.Update(Direction, Walk);
    }

    private void AnimationResolve()
    {
        var speed = Speed;
        speed = Speed * 10;
        if ((Position.X <= 0 && Direction == Enums.Direction.WalkLeft)
            || (Position.X >= Globals.Size.Width - Size.X && Direction == Enums.Direction.WalkRight)) Walk = false;

        if (Walk)
        {
            if (Direction == Enums.Direction.StandLeft)
            {
                Position += new Vector2(-speed, 0);
            }
            else if (Direction == Enums.Direction.StandRight)
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

        if (animRight.EndLoop && spriteType == SpriteType.SwordEffect)
        {            
            Direction = Enums.Direction.StandRight;
            Active = false;
        }

        if (animLeft.EndLoop && spriteType == SpriteType.SwordEffect)
        {
            Direction = Enums.Direction.StandLeft;
            Active = false;
        }
    }

    private void FallingResolve()
    {
        if (!Ground && FallingSpeed <= Globals.Gravity)
        {
            FallingSpeed += 0.05f;
            Position += new Vector2(0, FallingSpeed);
            if (FallingSpeed >= Globals.Gravity) FallingSpeed = Globals.Gravity;
        }
        else
        {
            FallingSpeed = -2;
        }

        if (Position.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel);
            Ground = true;
            if(spriteType == SpriteType.ArrowEffect) Active = false;
        }
        else
        {
            Ground = false;
        }
    }

    public void InitialPosition()
    {
        var oCenterX = Owner.Position.X + Owner.Size.X /2;
        var oCenterY = Owner.Position.Y + Owner.Size.Y / 2;
        var centerX = Size.X / 2;
        var centerY = Size.Y / 2;

        Position = new Vector2(oCenterX - centerX, oCenterY - centerY);

        if (Owner.GetSide() == Enums.Side.Right) Position += new Vector2(AttributeFX.Range, 0);
        if (Owner.GetSide() == Enums.Side.Left) Position -= new Vector2(AttributeFX.Range, 0);
    }

    public void CenterX_Adjust()
    {
        Position = new Vector2(Owner.RelativeX, Owner.Position.Y);
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
        )
        {
            return Enums.Side.Right;
        }

        if (Direction == Enums.Direction.StandLeft
            || Direction == Enums.Direction.WalkLeft
        )
        {
            return Enums.Side.Left;
        }

        return Enums.Side.None;
    }

    public bool GetCombo()
    {
        return combo;
    }

    public void SetCombo(bool combo)
    {
        this.combo = combo;
    }

    public void Draw()
    {
        _anims.Draw(Position);
    }
}
