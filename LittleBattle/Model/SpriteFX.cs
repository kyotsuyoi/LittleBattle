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
    public bool Active { get; set; }

    protected Texture2D texture;
    public Vector2 Position { get; set; }
    private Vector2 Size { get; set; }
    public float Speed { get; set; }
    public bool Walk { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public Vector2 Direction { get; set; }
    private Enums.Side Side { get; set; }
    public Enums.SpriteType spriteType { get; }
    public float RelativeX { get; set; }

    private readonly AnimationManager _anims = new AnimationManager();
    public Sprite Owner { get; set; }
    public AttributeFX AttributeFX { get; set; }

    private List<int> DamagedIDs;

    private bool combo = false;

    private float deadAlpha = 1f;
    private bool dead = false;

    protected Texture2D debugArea;
    private Collision collision;

    public SpriteFX(Sprite Owner, Side side, SpriteType spriteType, GraphicsDeviceManager graphics)
    {
        Active = true;
        this.Owner = Owner;
        this.spriteType = spriteType;
        Active = true;

        AttributeFX = new AttributeFX(spriteType);

        Speed = 1;

        Ground = false;
        FallingSpeed = 0;
        Direction = Enums.Direction.StandRight;
        if (side == Side.Left) Direction = Enums.Direction.StandLeft;
        AttributeFX.Range += Owner.Attribute.Range;

        debugArea = new Texture2D(graphics.GraphicsDevice, 1, 1);
        debugArea.SetData(new Color[] { Color.Blue });

        collision = new Collision();

        SetTexture();
        GroundLevel = Globals.GroundLevel + Size.Y - 20;

        CenterX_Adjust();
        InitialPosition();
        DamagedIDs = new List<int>();        
    }

    private void SetTexture()
    {
        int framesX = 12;
        int framesY = 1;
        if (spriteType == SpriteType.SwordEffect) { 
            texture = Globals.Content.Load<Texture2D>("Sprite/SwordEffect"); 
        }

        if (spriteType == SpriteType.ArrowEffect)
        {
            Walk = true;
            texture = Globals.Content.Load<Texture2D>("Sprite/ArrowEffectSmall01"); 
            framesX = 1; 
            framesY = 1;
            Texture2D texture2 = Globals.Content.Load<Texture2D>("Sprite/ArrowEffectSmall02");
            _anims.AddAnimation(Enums.Direction.DeadRight, new Animation(texture2, framesX, framesY, 0, 1, 0.01f, 1, false, false));
            _anims.AddAnimation(Enums.Direction.DeadLeft, new Animation(texture2, framesX, framesY, 0, 1, 0.01f, 1, true, false));

            FallingSpeed = 2;
        }

        _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));
        _anims.AddAnimation(Enums.Direction.WalkRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.WalkLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));
        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
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
        if (dead)
        {            
            deadAlpha -= 0.1f * Globals.ElapsedSeconds;
            if (deadAlpha <= 0) Active = false;            

            if (GetSide() == Side.Right && spriteType == SpriteType.ArrowEffect)
            {
                Direction = Enums.Direction.DeadRight;
                GroundLevel = Globals.GroundLevel - 20; 
            }

            if (GetSide() == Side.Left && spriteType == SpriteType.ArrowEffect)
            {
                Direction = Enums.Direction.DeadLeft;
                GroundLevel = Globals.GroundLevel - 20;
            }
            return;
        }

        var speed = Speed * 10;
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
        //else
        //{
        //    FallingSpeed = -200;
        //}

        if (Position.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel);
            Ground = true;
            if(spriteType == SpriteType.ArrowEffect) dead = true;
        }
        else
        {
            Ground = false;
        }
    }

    public void InitialPosition()
    {
        var oCenterX = Owner.Position.X + Owner.GetSize().X /2;
        var oCenterY = Owner.Position.Y + Owner.GetSize().Y / 2;
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
        var collide = collision.SquareCollision(Position, Size, target.Position, target.GetSize());
        if (collide && !DamagedIDs.Any(id=> id == target.ID))
        {
            target.TakeDamage(this);
            DamagedIDs.Add(target.ID);
        }
    }

    public void DamageObject(SpriteObject target)
    {
        var collide = collision.SquareCollision(Position, Size, target.Position, target.GetSize());
        if (collide && !DamagedIDs.Any(id => id == target.ID))
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
    
    private Rectangle GetRectangle()
    {
        var Pos = new Point((int)Position.X, (int)Position.Y);
        var Siz = new Point((int)Size.X, (int)Size.Y);

        return new Rectangle(Pos, Siz);
    }

    public Vector2 GetSize()
    {
        return Size;
    }

    public bool IsDead()
    {
        return dead;
    }

    public void SetFallingSpeed(float fallingSpeed)
    {
        FallingSpeed = fallingSpeed;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, float layerDepth)
    {
        if (Globals.Debug && Globals.DebugArea)
        {
            spriteBatch.Draw(debugArea, GetRectangle(), Color.Red * 0.4f);
        }

        _anims.Draw(Position, 1f, deadAlpha);
    }
}
