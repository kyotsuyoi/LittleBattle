using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

public class Sprite
{
    public int ID { get; }
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

    public Enums.Team Team;

    public float BotPatrol;

    public float BotPatrolWait;

    private bool combo = false;

    public Sprite(int ID, Vector2 position, Enums.SpriteType spriteType, Texture2D texture, int framesX, int framesY, Enums.Team team)
    {
        this.ID = ID;
        Position = position;
        this.spriteType = spriteType;
        Attribute = new Attribute();
        Team = team;

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
        _anims.AddAnimation(Enums.Direction.AttackRight, new Animation(texture, framesX, framesY, 0, 3, 0.05f, 3, false, false));
        _anims.AddAnimation(Enums.Direction.AttackLeft, new Animation(texture, framesX, framesY, 0, 3, 0.05f, 3, true, false));
        _anims.AddAnimation(Enums.Direction.DeadRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 4, false, false));
        _anims.AddAnimation(Enums.Direction.DeadLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 4, true, false));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        GroundLevel = Globals.Size.Height - Size.Y - 30;
        spriteFXs = new List<SpriteFX>();
        Team = team;

        //Debug
        //if (ID == 1)
        //{
        //    Attribute.BaseHP = 200;
        //    Attribute.BaseJumpPower = 20;
        //    Attribute.Attack = 10;
        //    Attribute.Defense = 10;
        //    Attribute.BaseAttackCooldown = 0.02f;
        //}
    }

    public void Update()
    {
        AnimationResolve();
        AttackResolve();
        UpdateCooldown();
        JumpResolve();
        FallingResolve();

        if (spriteType != Enums.SpriteType.None)
        {
            Position += new Vector2(Globals.CameraMovement,0);
        }
        RelativeX = Position.X - Globals.GroundX;

        _anims.Update(Direction, Walk);
    }

    private void AnimationResolve()
    {
        if (IsDead()) return;
        var speed = Attribute.Speed;
        if (spriteType != Enums.SpriteType.Player1) speed = Attribute.Speed * 2;
        if ((Position.X <= 0 && GetSide() == Enums.Side.Left)
            || (Position.X >= Globals.Size.Width - Size.X && GetSide() == Enums.Side.Right)) Walk = false;

        //Cameraman ID
        if (ID == 0) speed = 0;

        if (Walk)
        {
            if (GetSide() == Enums.Side.Left /*&& Globals.NegativeLimit.Width < RelativeX*/)
            {
                Position += new Vector2(-speed, 0);
            }
            else if (GetSide() == Enums.Side.Right /*&& Globals.PositiveLimit.Width > RelativeX + Size.X*/)
            {
                Position += new Vector2(speed, 0);
            }
        }
        else
        {
            if (GetSide() == Enums.Side.Left )
            {
                Direction = Enums.Direction.StandLeft;
            }
            else if (GetSide() == Enums.Side.Right)
            {
                Direction = Enums.Direction.StandRight;
            }
        }

        if(Attribute.Knockback > 0)
        {
            if(Attribute.KnockbackSide == Enums.Side.Right)
            {
                Position += new Vector2(Attribute.Knockback, 0);
            }
            if (Attribute.KnockbackSide == Enums.Side.Left)
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
            Attribute.JumpPower = Attribute.BaseJumpPower;
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
            if (GetSide() == Enums.Side.Right)
            {
                Direction = Enums.Direction.AttackRight;
            }
            if (GetSide() == Enums.Side.Left)
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

        foreach (var attack in spriteFXs)
        {
            attack.Update();
        }
        spriteFXs = spriteFXs.Where(attack => attack.Active == true).ToList();
    }

    public float CameraDirectionSpeed()
    {
        var value = Attribute.Knockback;
        if(Attribute.KnockbackSide == Enums.Side.Right)
        {
            value = -value;
        }
        if (IsDead()) return 0;
        if (GetSide() == Enums.Side.Left && Walk && !(RelativeX < Globals.NegativeLimit.Width)) value = Attribute.Speed + value;
        if (GetSide() == Enums.Side.Right && Walk && !(RelativeX + Size.X > Globals.PositiveLimit.Width)) value = -Attribute.Speed + value;
        return value;
    }

    public void SetMovement(bool move, Enums.Side side)
    {
        if (IsDead()) return;

        var PositiveLimit = Globals.PositiveLimit.Width;
        var NegativeLimit = Globals.NegativeLimit.Width;

        if (ID == 0)
        {
            PositiveLimit = Globals.PositiveLimit.Width - Globals.Size.Width / 2;
            NegativeLimit = Globals.NegativeLimit.Width + Globals.Size.Width / 2;
        }

        if (move){
            if(side == Enums.Side.Right && !Attack && RelativeX + Size.X < PositiveLimit)
            {
                Direction = Enums.Direction.WalkRight;
                Walk = true;
            }
            else if (side == Enums.Side.Left && !Attack && RelativeX > NegativeLimit)
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
            Walk = false;
        }
    }

    public void SetAttack()
    {
        if (Attribute.HP <= 0) return;
        if (Attribute.AttackCooldown > 0) return; 
        spriteFXs.Add(new SpriteFX(this, GetSide(), Enums.SpriteType.SwordAttack, Globals.Content.Load<Texture2D>("Sprites/SwordEffect"), 12, 1));
        if (combo)
        {
            spriteFXs[spriteFXs.Count() - 1].AttributeFX.Damage += 20;
            spriteFXs[spriteFXs.Count() - 1].AttributeFX.Range += 100;
            spriteFXs[spriteFXs.Count() - 1].AttributeFX.Knockback += 5;
            spriteFXs[spriteFXs.Count() - 1].InitialPosition();
            combo = false;
        }
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
        if (res < 1) res = 1;
        if (Owner.spriteType == Enums.SpriteType.Player1 && Owner.combo == false) { 
            Owner.combo = true;
            Owner.Attribute.AttackCooldown = 0.1f;
        }

        Attribute.HP -= res;
        if(IsDead()) return;

        Attribute.Knockback = spriteFX.AttributeFX.Knockback;
        Attribute.KnockbackSide = spriteFX.GetSide();
        if (spriteType == Enums.SpriteType.Player1)
        {
            Attribute.Knockback = Attribute.Knockback / 2;
        }
    }

    public void UpdateSpriteFXDamage(List<Sprite> targets)
    {
        var inner_targets = targets.Where(target => target.Team != Team).ToList();
        foreach (var target in inner_targets)
        {
            foreach (var damage in spriteFXs)
            {
                damage.Damage(target);
            }
        }        
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

    public void Revive()
    {
        if (!IsDead()) return;
        Attribute.HP = Attribute.BaseHP;
        var animRight = _anims.GetAnimation(Enums.Direction.DeadRight);
        var animLeft = _anims.GetAnimation(Enums.Direction.DeadLeft);

        if (animRight.EndLoop)
        {
            animRight.Reset();
            Direction = Enums.Direction.StandRight;
        }

        if (animLeft.EndLoop)
        {
            animLeft.Reset();
            Direction = Enums.Direction.StandLeft;
        }
    }

    public bool IsDead()
    {
        if(Attribute.HP <= 0)
        {
            Dead();
            return true;
        }
        return false;
    }

    private void Dead() {
        Attribute.HP = 0;
        spriteFXs = new List<SpriteFX>();

        if (GetSide() == Enums.Side.Right)
        {
            Direction = Enums.Direction.DeadRight;
        }
        if (GetSide() == Enums.Side.Left)
        {
            Direction = Enums.Direction.DeadLeft;
        }
        Walk = false;
        Attack = false;
        Attribute.Knockback = 0;
        combo = false;
    }

    public int GetSpriteFXCount()
    {
        return spriteFXs.Count();
    }

    public void CenterX_Adjust()
    {
        Position = new Vector2(Position.X - Size.X / 2, Position.Y);
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        //if (ID == 0) return;
        _anims.Draw(Position);

        foreach (var attack in spriteFXs)
        {
            attack.Draw();
        }

        if (IsDead() || ID==0) return;
        if(ID==01) spriteBatch.DrawString(font, "*", new Vector2(Position.X+18, Position.Y-20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 12, Position.Y-2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
    }
}
