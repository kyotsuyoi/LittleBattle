using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

public class Sprite
{
    protected Texture2D texture;
    public Vector2 Position { get; set; }
    public Vector2 Size { get; }
    public bool Walk { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Attack { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public Vector2 Direction { get; set; }
    public Enums.SpriteType spriteType { get; }
    public float RelativeX { get; set; }

    private List<SpriteFX> spriteFXs;

    private readonly AnimationManager _anims = new AnimationManager();
    public Attribute Attribute { get; set; }

    public Sprite(Vector2 position, Enums.SpriteType spriteType, Texture2D texture, int framesX, int framesY)
    {
        Position = position;
        this.spriteType = spriteType;
        Attribute = new Attribute();

        if (spriteType == Enums.SpriteType.Bot) Attribute.Speed = 0.5f;

        Jump = false;
        Ground = false;
        FallingSpeed = 0;
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
        spriteFXs = new List<SpriteFX>();
    }

    public void Update()
    {
        AnimationResolve();
        AttackResolve();
        UpdateCooldown();
        JumpResolve();
        FallingResolve();

        if (spriteType != Enums.SpriteType.Player1)
        {
            Position += new Vector2(Globals.CameraMovement,0);
        }
        RelativeX = Position.X - Globals.GroundX;

        foreach(var attack in spriteFXs)
        {
            attack.Update();
        }
        spriteFXs = spriteFXs.Where(attack => attack.Active == true).ToList();

        _anims.Update(Direction, Walk);
    }

    private void AnimationResolve()
    {
        var speed = Attribute.Speed;
        if (spriteType != Enums.SpriteType.Player1) speed = Attribute.Speed * 2;
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

        if(Attribute.Knockback > 0)
        {
            if(Attribute.KnockbackSide == Enums.Direction.StandRight)
            {
                Position += new Vector2(Attribute.Knockback, 0);
            }
            if (Attribute.KnockbackSide == Enums.Direction.StandLeft)
            {
                Position += new Vector2(-Attribute.Knockback, 0);
            }
            Attribute.Knockback-= 0.5f;
            if (Attribute.Knockback <= 0) Attribute.Knockback = 0;
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

        Ground = false;
        if (Position.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel);
            Attribute.JumpPower = 5;
            Ground = true;
        }
    }

    private void JumpResolve()
    {
        if (Jump)
        {
            Ground = false;
            Position += new Vector2(0, -Attribute.JumpPower);
            Attribute.JumpPower -= 0.25f;
            if (Attribute.JumpPower <= 0)
            {
                Attribute.JumpPower = 0;
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
        }        

        var animRight = _anims.GetAnimation(Enums.Direction.AttackRight);
        var animLeft = _anims.GetAnimation(Enums.Direction.AttackLeft);

        if (animRight.EndLoop && Attack)
        {
            animRight.Reset();
            Attack = false;
            Direction = Enums.Direction.StandRight;
        }

        if (animLeft.EndLoop && Attack)
        {
            animLeft.Reset();
            Attack = false; 
            Direction = Enums.Direction.StandLeft;
        }
    }

    public float DirectionSpeed()
    {
        var value = Attribute.Knockback;
        if(Attribute.KnockbackSide == Enums.Direction.StandRight)
        {
            value = -value;
        }
        if (Direction == Enums.Direction.WalkLeft && Walk) value = Attribute.Speed + value;
        if (Direction == Enums.Direction.WalkRight && Walk) value = -Attribute.Speed + value;
        return value;
    }

    public void SetMovement(Vector2 Direction)
    {
        if (Attribute.HP <= 0/* && spriteType != Enums.SpriteType.Player1*/)
        {
            this.Direction = Enums.Direction.None;
            return;
        }
        this.Direction = Direction;
        Walk = true;
    }

    public void SetAttack()
    {
        if (Attribute.HP <= 0 /*&& spriteType != Enums.SpriteType.Player1*/) return;
        if (Attribute.AttackCooldown > 0) return; 
        spriteFXs.Add(new SpriteFX(this, Direction, Enums.SpriteType.SwordAttack, Globals.Content.Load<Texture2D>("Sprites/SwordEffect"), 12, 1));
        Attack = true;
        Attribute.AttackCooldown = Attribute.BaseAttackCooldown;
    }

    public void SetJump()
    {
        if (Attribute.HP <= 0 /*&& spriteType != Enums.SpriteType.Player1*/) return;
        Jump = true;
    }

    private void UpdateCooldown()
    {
        Attribute.AttackCooldown-=Globals.ElapsedSeconds;
        if (Attribute.AttackCooldown < 0) Attribute.AttackCooldown = 0;
    }

    public void TakeDamage(SpriteFX spriteFX)
    {
        var Owner = spriteFX.Owner;
        var res = (Owner.Attribute.Attack + spriteFX.AttributeFX.Damage) - Attribute.Defense;

        Attribute.Knockback = spriteFX.AttributeFX.Knockback;
        Attribute.KnockbackSide = spriteFX.Direction;
        if (spriteType == Enums.SpriteType.Player1)
        {
            Attribute.Knockback = Attribute.Knockback / 2;
        }

        Attribute.HP -= res;
        if (Attribute.HP < 0) Attribute.HP = 0;
    }

    public void UpdateSpriteFXDamage(List<Sprite> targets)
    {
        foreach(var target in targets)
        {
            foreach (var damage in spriteFXs)
            {
                damage.Damage(target);
            }
        }        
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        _anims.Draw(Position);

        foreach (var attack in spriteFXs)
        {
            attack.Draw();
        }

        spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 12, Position.Y-2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
    }
}
