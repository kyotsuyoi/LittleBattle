using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static LittleBattle.Classes.Enums;

public class Sprite
{
    public bool Active { get; set; }
    public int ID { get; }
    protected Texture2D texture;
    public Vector2 Position { get; set; }
    public Vector2 CenterPosition { get; set; }

    //Needs change to Size Type
    private Vector2 Size { get; set; }
    public bool Walk { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Attack { get; set; }
    public bool Ground { get; set; }
    private bool Combo { get; set; }
    public bool Climb { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public Vector2 Direction { get; set; }
    public SpriteType spriteType { get; }
    public ClassType classType { get; }
    public float RelativeX { get; set; }

    private List<SpriteFX> spriteFXs;
    private List<SpriteObject> spriteObjects;

    private readonly AnimationManager _anims = new AnimationManager();
    public Attribute Attribute { get; set; }

    public Team Team;

    public bool BotPatrol = false;
    public float BotPatrolX;
    public float BotPatrolX_Area;
    public float BotPatrolWait;
    public bool BotGoTo = false;
    private float deadAlpha = 1f;

    public Sprite(int ID, Vector2 position, SpriteType spriteType, Team team, ClassType classType)
    {
        Active = true;
        this.ID = ID;
        Position = position;
        this.spriteType = spriteType;
        this.classType = classType;
        Attribute = new Attribute(classType);
        Team = team;

        if (spriteType == SpriteType.Bot) Attribute.Speed = 0.5f;
        if (spriteType == SpriteType.Player2) Attribute.Speed /=2;

        Walk = false;
        Run = false;
        Jump = false;
        Attack = false;
        Ground = false;
        Combo = false;
        Climb = false;
        FallingSpeed = 0;
        Direction = Enums.Direction.StandRight;

        SetTexture();

        spriteFXs = new List<SpriteFX>();
        spriteObjects = new List<SpriteObject>();
        CalcGroundLevel();
    }

    private void SetTexture()
    {
        int framesX = 4;
        int framesY = 6;

        if (spriteType == SpriteType.Cameraman)
        {
            framesY = 4;
            texture = Globals.Content.Load<Texture2D>("Sprites/SpriteCameraman_x3");
        }
        else if (Team == Team.Team1)
        {
            if (classType == ClassType.Warrior) texture = Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3");
            if (classType == ClassType.Archer) texture = Globals.Content.Load<Texture2D>("Sprites/Sprite03_x3");
        }
        else if (Team == Team.Team2)
        {
            if (classType == ClassType.Warrior) texture = Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3");
            if (classType == ClassType.Archer) texture = Globals.Content.Load<Texture2D>("Sprites/Sprite04_x3");
        }

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

    public void Update()
    {
        //UpdateInteraction(); 
        AnimationResolve();
        AttackResolve();
        UpdateCooldown();
        JumpResolve();
        FallingResolve();

        if (spriteType != SpriteType.Cameraman)
        {
            Position += new Vector2(Globals.CameraMovement,0);
        }
        RelativeX = Position.X - Globals.GroundX;

        if (Combo && Attribute.ComboTimeLimit == 0)
        {
            Combo = false;
        }

        _anims.Update(Direction, Walk);
    }

    private void AnimationResolve()
    {
        if (IsDead())
        {
            if(spriteType == SpriteType.Bot)
            {
                deadAlpha -= 0.1f * Globals.ElapsedSeconds;
                if (deadAlpha <= 0) Active = false;
            }
            return;
        }

        var speed = Attribute.Speed;
        if (spriteType != SpriteType.Player1) speed = Attribute.Speed * 2;
        //if ((Position.X <= 0 && GetSide() == Side.Left)
        //    || (Position.X >= Globals.Size.Width - Size.X && GetSide() == Side.Right)) Walk = false;

        //Cameraman ID
        if (ID == 0) speed = 0;

        if (Walk)
        {
            if (GetSide() == Side.Left /*&& Globals.NegativeLimit.Width < RelativeX*/)
            {
                Position += new Vector2(-speed, 0);
            }
            else if (GetSide() == Side.Right /*&& Globals.PositiveLimit.Width > RelativeX + Size.X*/)
            {
                Position += new Vector2(speed, 0);
            }
        }
        else
        {
            if (GetSide() == Side.Left )
            {
                Direction = Enums.Direction.StandLeft;
            }
            else if (GetSide() == Side.Right)
            {
                Direction = Enums.Direction.StandRight;
            }
        }

        if (Climb)
        {
            if (GetSide() == Side.Up)
            {
                Direction = Enums.Direction.ClimbUp;
                Position += new Vector2(0, -speed);
            }
            else if (GetSide() == Side.Down)
            {
                Direction = Enums.Direction.ClimbDown;
                Position += new Vector2(0, speed);
            }
            else
            {
                _anims.Update(Enums.Direction.ClimbUp, false, 1);
            }
        }

        if(Attribute.Knockback > 0)
        {
            if(Attribute.KnockbackSide == Side.Right)
            {
                Position += new Vector2(Attribute.Knockback, 0);
            }
            if (Attribute.KnockbackSide == Side.Left)
            {
                Position += new Vector2(-Attribute.Knockback, 0);
            }
            Attribute.Knockback-= 0.5f;
            if (Attribute.Knockback <= 0) Attribute.Knockback = 0;
        }

        if(Attribute.StuntTime > 0)
        {
            if (GetSide() == Side.Right)
            {
                Direction = Enums.Direction.StuntRight;
            }
            if (GetSide() == Side.Left)
            {
                Direction = Enums.Direction.StuntLeft;
            }
        }
        else
        {
            var animRight = _anims.GetAnimation(Enums.Direction.StuntRight);
            var animLeft = _anims.GetAnimation(Enums.Direction.StuntLeft);

            animRight.Reset();
            animLeft.Reset();
        }
    }

    private void FallingResolve()
    {
        if (Climb) return;
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
        if (Position.Y + Size.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel - Size.Y);
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
            if (GetSide() == Side.Right)
            {
                Direction = Enums.Direction.AttackRight;
            }
            if (GetSide() == Side.Left)
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
        if (IsDead()) return 0;
        if (GetSide() == Side.Left && Walk) return Attribute.Speed; 
        if (GetSide() == Side.Right && Walk) return -Attribute.Speed;
        return 0;
    }

    public void SetMovement(bool move, Side side)
    {
        if (IsDead() || Attribute.StuntTime > 0) {
            Walk = false;
            return; 
        }

        var PositiveLimit = Globals.PositiveLimit.Width;
        var NegativeLimit = Globals.NegativeLimit.Width;

        //Cameraman only
        if (ID == 0)
        {
            PositiveLimit = Globals.PositiveLimit.Width - Globals.Size.Width / 2;
            NegativeLimit = Globals.NegativeLimit.Width + Globals.Size.Width / 2;
        }

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

        if (move){
            if(side == Side.Right && RelativeX + Size.X/2 < PositiveLimit)
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
            Walk = false;
        }
    }

    public void SetAttack()
    {
        if (Climb) return;
        //if (IsDead() || Attribute.StuntTime > 0) return;
        if (Attribute.AttackCooldown > 0) return; 

        if (classType == ClassType.Warrior) spriteFXs.Add(new SpriteFX(this, GetSide(), SpriteType.SwordEffect));
        if (classType == ClassType.Archer) spriteFXs.Add(new SpriteFX(this, GetSide(), SpriteType.ArrowEffect));

        if (Combo)
        {
            var spFX = spriteFXs[spriteFXs.Count() - 1];
            spFX.InitialPosition();
            Combo = false;

            //Jump to position into combo
            if (spFX.spriteType == SpriteType.SwordEffect)
            {
                if (spFX.GetSide() == Side.Right) Position = new Vector2(spFX.Position.X + spFX.GetSize().X / 2, spFX.Position.Y);
                if (spFX.GetSide() == Side.Left) Position = new Vector2(spFX.Position.X, spFX.Position.Y);

                spFX.AttributeFX.Damage += 8;
                spFX.AttributeFX.Range += 8;
                spFX.AttributeFX.Knockback += 4;
                spFX.AttributeFX.StuntTime = 1f;
            }

            if (spFX.spriteType == SpriteType.ArrowEffect)
            {
                spFX.AttributeFX.Damage += 4;
                spFX.AttributeFX.Range += 0;
                spFX.AttributeFX.Knockback += 1;
                spFX.AttributeFX.StuntTime = 0.5f;
            }

            spFX.SetCombo(true);            
        }

        Attack = true;
        Attribute.AttackCooldown = Attribute.BaseAttackCooldown;
    }

    public void SetJump()
    {
        //if (IsDead()) return;
        Jump = true;
        if (Climb)
        {
            SetRandomSide();
            Climb = false;
        }
    }

    private void SetRandomSide()
    {
        System.Random random = new System.Random();
        var randomVal = random.Next(9);

        if (randomVal >= 5)
        {
            Direction = Enums.Direction.StandRight;
        }
        else
        {
            Direction = Enums.Direction.StandLeft;
        }
    }

    private void UpdateCooldown()
    {
        Attribute.AttackCooldown -= Globals.ElapsedSeconds;
        if (Attribute.AttackCooldown < 0) Attribute.AttackCooldown = 0;

        Attribute.ComboTimeLimit -= Globals.ElapsedSeconds;
        if (Attribute.ComboTimeLimit < 0 || !Combo) Attribute.ComboTimeLimit = 0;

        Attribute.StuntTime -= Globals.ElapsedSeconds;
        if (Attribute.StuntTime < 0) Attribute.StuntTime = 0;
    }

    public void TakeDamage(SpriteFX spriteFX)
    {
        if (!spriteFX.Active) return;
        var Owner = spriteFX.Owner;
        var res = (Owner.Attribute.Attack + spriteFX.AttributeFX.Damage + Owner.Attribute.BuffAttack) - Attribute.Defense;
        if (res < 1) res = 1;

        Attribute.HP -= res;
        //if(IsDead()) return;

        if (Owner.spriteType == SpriteType.Player1 || Owner.spriteType == SpriteType.Player2
            && !Owner.Combo && !spriteFX.GetCombo())
        {
            Owner.Attribute.AttackCooldown = Owner.Attribute.AttackCooldown / 3;
            Owner.Attribute.ComboTimeLimit = Owner.Attribute.BaseComboTimeLimit;
            Owner.Combo = true;
        }

        if (Owner.spriteType == SpriteType.Bot && !Owner.Combo && !spriteFX.GetCombo())
        {
            Owner.Attribute.ComboTimeLimit = Owner.Attribute.BaseComboTimeLimit;
            Owner.Combo = true;
        }

        Attribute.StuntTime = spriteFX.AttributeFX.StuntTime;
        Attribute.Knockback = spriteFX.AttributeFX.Knockback + Owner.Attribute.BuffKnockback;
        Attribute.KnockbackSide = spriteFX.GetSide();
        if (spriteType == SpriteType.Player1 || Owner.spriteType == SpriteType.Player2)
        {
            Attribute.Knockback = Attribute.Knockback / 2;
        }

        if (Attribute.Knockback > 0 && Climb) { Climb = false; SetRandomSide(); }

        if (spriteFX.spriteType == SpriteType.ArrowEffect) spriteFX.Active = false;
    }

    public void UpdateSpriteFXDamage(List<Sprite> targets)
    {
        //var inner_targets = targets.Where(target => target.Team != Team && !target.IsDead()).ToList();
        foreach (var damage in spriteFXs)
        {
            foreach (var target in targets)
            {
                if (!target.IsDead() && target.Team != Team && !damage.IsDead()) damage.Damage(target);
                foreach (var _object in target.spriteObjects)
                {
                    if (target.Team != Team) damage.DamageObject(_object);
                }
            }
        }
    }

    public void UpdateSpriteObjects()
    {
        foreach (var _object in spriteObjects)
        {
            _object.Update();
        }
    }

    public Side GetSide()
    {
        if (Direction == Enums.Direction.StandRight
            || Direction == Enums.Direction.WalkRight
            || Direction == Enums.Direction.AttackRight
            || Direction == Enums.Direction.DeadRight
            || Direction == Enums.Direction.StuntRight
        )
        {
            return Side.Right;
        }

        if (Direction == Enums.Direction.StandLeft 
            || Direction == Enums.Direction.WalkLeft
            || Direction == Enums.Direction.AttackLeft
            || Direction == Enums.Direction.DeadLeft
            || Direction == Enums.Direction.StuntLeft
        )
        {
            return Side.Left;
        }

        if (Direction == Enums.Direction.ClimbUp)
        {
            return Side.Up;
        }

        if (Direction == Enums.Direction.ClimbDown)
        {
            return Side.Down;
        }

        return Side.None;
    }

    public void SetSide(Side side)
    {
        if (side == Side.Left)
        {
            if (Direction == Enums.Direction.StandLeft)
            {
                Position = Enums.Direction.StandRight;
            }
            else if (Direction == Enums.Direction.WalkLeft)
            {
                Position = Enums.Direction.WalkRight;
            }
            else if (Direction == Enums.Direction.AttackLeft)
            {
                Position = Enums.Direction.AttackRight;
            }
            else if (Direction == Enums.Direction.DeadLeft)
            {
                Position = Enums.Direction.DeadRight;
            }
            else if (Direction == Enums.Direction.StuntLeft)
            {
                Position = Enums.Direction.StuntRight;
            }
        }

        if (side == Side.Right)
        {
            if (Direction == Enums.Direction.StandRight)
            {
                Position = Enums.Direction.StandLeft;
            }
            else if (Direction == Enums.Direction.WalkRight)
            {
                Position = Enums.Direction.WalkLeft;
            }
            else if (Direction == Enums.Direction.AttackRight)
            {
                Position = Enums.Direction.AttackLeft;
            }
            else if (Direction == Enums.Direction.DeadRight)
            {
                Position = Enums.Direction.DeadLeft;
            }
            else if (Direction == Enums.Direction.StuntRight)
            {
                Position = Enums.Direction.StuntLeft;
            }
        }
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

        deadAlpha = 1f;
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

        var side = GetSide();
        if (side == Side.Right) Direction = Enums.Direction.DeadRight;
        if (side == Side.Left) Direction = Enums.Direction.DeadLeft;
        if (side == Side.Up) SetRandomSide();
        if (side == Side.Down) SetRandomSide();
        if (side == Side.None) SetRandomSide();

        Walk = false;
        Attack = false;
        Climb = false;
        Attribute.Knockback = 0;
        Combo = false;
        Jump = false;
    }

    public int GetSpriteFXCount()
    {
        return spriteFXs.Count();
    }

    public void CenterX_Adjust()
    {
        Position = new Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
    }

    public bool GetCombo()
    {
        return Combo;
    }

    public void SetToGroundLevel(float positionX)
    {
        //Position = new Vector2(Position.X, GroundLevel);
    }

    public void SetInitialPatrolArea(float positionX)
    {
        BotPatrolX_Area = positionX;
        BotPatrol = true;
    }

    public void SetObject(SpriteType spriteType)
    {
        //spriteObjects = new List<SpriteObject>();
        spriteObjects.Add(new SpriteObject(this, GetSide(), spriteType));
        //var _object = spriteObjects[spriteObjects.Count - 1];
        //_object.CenterX_Adjust();
        //_object.SetToGroundLevel(0);
    }

    public void InteractObjects(List<Sprite> players, List<Sprite> bots)
    {
        var _object = GetInteractObject(players, bots);

        if (_object != null)
        {
            if (_object.spriteType == SpriteType.ArcherTower)
            {
                Climb = true;
                Walk = false;
                Position = new Vector2(_object.Position.X + _object.GetSize().X/2 - Size.X / 2, Position.Y);
                //GroundLevel = (int)_object.Position.Y;
            }
            else
            {
                GroundLevel = Globals.GroundLevel;
            }
        }
        else
        {
            GroundLevel = Globals.GroundLevel;
        }
    }

    public void UpdateInteraction(List<Sprite> players, List<Sprite> bots)
    {
        spriteObjects = spriteObjects.Where(_spriteobject => _spriteobject.Active).ToList();

        var _object = GetInteractObject(players, bots);

        if (_object != null && GroundLevel < (int)_object.Position.Y)
        {
            GroundLevel = (int)(_object.Position.Y);
        }

        var test = (int)(Position.Y + Size.Y);
        if (_object != null && Climb && (int)(Position.Y + Size.Y -4) <= (int)_object.Position.Y)
        {
            GroundLevel = (int)(_object.Position.Y);
            Climb = false;
            SetRandomSide();
        }

        //Needs buff list
        if (_object != null && (int)(_object.Position.Y) == GroundLevel /*&& classType == ClassType.Archer*/)
        {
            Climb = false;
            Attribute.BuffAttack = 5;
            Attribute.BuffKnockback = 2;
        }
        else
        {
            Attribute.BuffAttack = 0;
            Attribute.BuffKnockback = 0;
        }

        //var nCalc = (int)(_object.Position.Y);
        if (_object == null || (int)(_object.Position.Y) != GroundLevel)
        {
            GroundLevel = Globals.GroundLevel ;
        }
    }

    private SpriteObject GetInteractObject(List<Sprite> players, List<Sprite> bots)
    {
        Collision collision = new Collision();
        List<SpriteObject> localObjects = new List<SpriteObject>();

        foreach (var player in players)
        {
            foreach (var p_object in player.spriteObjects)
            {
                localObjects.Add(p_object);
            }
        }

        foreach (var bot in bots)
        {
            foreach (var b_object in bot.spriteObjects)
            {
                localObjects.Add(b_object);
            }
        }

        SpriteObject _object = null;
        localObjects = localObjects.Where(_spriteobject => !_spriteobject.IsDead()).ToList();
        foreach (var local_object in localObjects)
        {
            var PosB = new Point((int)local_object.Position.X, (int)local_object.Position.Y);
            var SizB = new Point((int)local_object.GetSize().X, (int)local_object.GetSize().Y);
            var RectB = new Rectangle(PosB, SizB);
            var isCollide = collision.IsCollide(GetRectangle(), RectB);

            if (isCollide)
            {
                _object = local_object;
                break;
            }
        }
        return _object;
    }

    private Rectangle GetRectangle()
    {
        var AdjustSizX = (int)(Size.X * 0.5);
        var AdjustPosX = (int)(Position.X + ((Size.X - AdjustSizX))/2);

        var AdjustSizY = (int)(Size.Y);//(int)(Size.Y * 0.9);
        var AdjustPosY = (int)(Position.Y);//(int)(Position.Y + ((Size.Y - AdjustSizY)) / 2);

        var Pos = new Point(AdjustPosX, AdjustPosY);
        var Siz = new Point(AdjustSizX, AdjustSizY);

        return new Rectangle(Pos, Siz);
    }

    private float CalcGroundLevel()
    {
        return GroundLevel = Globals.GroundLevel;
    }

    public Vector2 GetSize()
    {
        return Size;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics)
    {
        //if (ID == 0) return;

        _anims.Draw(Position, 0.9999f, deadAlpha);

        foreach (var attack in spriteFXs)
        {
            attack.Draw(spriteBatch, font, graphics, 0.1f);
        }

        if (ID == 01 || ID == 02) spriteBatch.DrawString(font, "*", new Vector2(Position.X + 18, Position.Y - 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);

        if (IsDead() || ID==0) return;
        string mark = "";
        if (BotPatrol)
        {
            mark += "P ";
        }
        if (BotGoTo)
        {
            mark += "G ";
        }

        spriteBatch.DrawString(font, mark, new Vector2(Position.X + 18, Position.Y - 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 12, Position.Y-2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        Texture2D _texture;
        if (Globals.Debug && Globals.DebugArea)
        {
            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Blue });
            spriteBatch.Draw(_texture, GetRectangle(), Color.Blue * 0.4f);
        }

        var _hp_percent = Attribute.HP * 100 / Attribute.BaseHP;
        if (_hp_percent >= 100) return;
        var hp_val = Size.X * _hp_percent / 100;

        _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
        _texture.SetData(new Color[] { Color.Black });
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X, 4), Color.Black * 0.6f);

        var _color = Color.GreenYellow;
        if (_hp_percent < 75) _color = Color.Yellow;
        if (_hp_percent < 50) _color = Color.Orange;
        if (_hp_percent < 25) _color = Color.Red;

        _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
        _texture.SetData(new Color[] { _color });
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)hp_val, 4), _color * 0.6f);

    }

    public void DrawObjects(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics)
    {
        foreach (var _object in spriteObjects)
        {
            _object.Draw(spriteBatch, font, graphics, 1);
        }
    }
}
