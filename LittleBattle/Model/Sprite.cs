using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    protected Vector2 Size { get; set; }

    public bool Walk { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Attack { get; set; }
    public bool Ground { get; set; }
    protected bool Combo { get; set; }
    public bool Climb { get; set; }
    public bool Work { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    public Vector2 Direction { get; set; }
    public SpriteType spriteType { get; }
    public ClassType classType { get; }
    public float RelativeX { get; set; }

    protected List<SpriteFX> spriteFXs;
    private List<SpriteObject> spriteObjects;

    protected readonly AnimationManager _anims = new AnimationManager();
    public LittleBattle.Model.Attribute Attribute { get; set; }

    public Team Team;

    protected float deadAlpha = 1f;

    public bool HoldUp = false;
    public bool HoldDown = false;

    private int WorkingID = 0;

    private Bag _Bag;
    private List<SpriteObject> objectsBuild;
    private IconDisplay IconDisplay;
    private List<IconDisplay> Icons;

    public Enums.Action SelectedAction = 0;

    public Sprite(int ID, Vector2 position, SpriteType spriteType, Team team, ClassType classType)
    {
        Active = true;
        this.ID = ID;
        Position = position;
        this.spriteType = spriteType;
        this.classType = classType;
        Attribute = new LittleBattle.Model.Attribute(classType);
        Team = team;

        if (spriteType == SpriteType.Bot) Attribute.Speed = 0.5f;
        if (spriteType == SpriteType.Player2) Attribute.Speed /= 2;

        Walk = false;
        Run = false;
        Jump = false;
        Attack = false;
        Ground = false;
        Combo = false;
        Climb = false;
        Work = false;
        FallingSpeed = 0;
        Direction = Enums.Direction.StandRight;

        SetTexture();

        spriteFXs = new List<SpriteFX>();
        spriteObjects = new List<SpriteObject>();
        objectsBuild = new List<SpriteObject>();
        Icons = new List<IconDisplay>();
        _Bag = new Bag();

        if (classType == ClassType.Worker)
        {
            _Bag.AddItem(SpriteType.Seed, 5);
        }
        CalcGroundLevel();
    }

    protected virtual void SetTexture()
    {
        int framesX = 4;
        int framesY = 6;

        if (Team == Team.Team1)
        {
            if (classType == ClassType.Warrior) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite01");
            if (classType == ClassType.Archer) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite03");
            if (classType == ClassType.Worker) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite05");
        }
        else if (Team == Team.Team2)
        {
            if (classType == ClassType.Warrior) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite02");
            if (classType == ClassType.Archer) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite04");
            if (classType == ClassType.Worker) texture = Globals.Content.Load<Texture2D>("Sprite_x3/Sprite06");
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

        _anims.AddAnimation(Enums.Direction.WorkRight, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 3, false, true));
        _anims.AddAnimation(Enums.Direction.WorkLeft, new Animation(texture, framesX, framesY, 0, 3, 0.1f, 3, true, true));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
    }

    public virtual void Update()
    {
        SetAnimationResolve();
        AttackResolve();
        UpdateCooldown();
        JumpResolve();
        FallingResolve();

        Position += new Vector2(Globals.CameraMovement, 0);
        RelativeX = Position.X - Globals.GroundX;

        if (Combo && Attribute.ComboTimeLimit == 0)
        {
            Combo = false;
        }

        _anims.Update(Direction, Walk);
    }

    protected virtual void SetAnimationResolve()
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

        var speed = Attribute.Speed;
        if (spriteType != SpriteType.Player1) speed = Attribute.Speed * 2;

        AnimationResolve(speed);
    }

    protected void AnimationResolve(float speed)
    {
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
            if (GetSide() == Side.Left)
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

        if (Work)
        {
            if (GetSide() == Side.Right)
            {
                Direction = Enums.Direction.WorkRight;
            }
            else if (GetSide() == Side.Left)
            {
                Direction = Enums.Direction.WorkLeft;
            }
        }

        if (Attribute.Knockback > 0)
        {
            if (Attribute.KnockbackSide == Side.Right)
            {
                Position += new Vector2(Attribute.Knockback, 0);
            }
            if (Attribute.KnockbackSide == Side.Left)
            {
                Position += new Vector2(-Attribute.Knockback, 0);
            }
            Attribute.Knockback -= 0.5f;
            if (Attribute.Knockback <= 0) Attribute.Knockback = 0;
        }

        if (Attribute.StuntTime > 0)
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

    protected void FallingResolve()
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
        float p = Position.Y + Size.Y;
        if (Position.Y + Size.Y >= GroundLevel)
        {
            Position = new Vector2(Position.X, GroundLevel - Size.Y);
            Attribute.JumpPower = Attribute.BaseJumpPower;
            Ground = true;
        }
    }

    protected void JumpResolve()
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

    protected void AttackResolve()
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

    public virtual void SetMovement(bool move, Side side)
    {
        if (!EnabledAction()) return;

        var PositiveLimit = Globals.PositiveLimit.Width;
        var NegativeLimit = Globals.NegativeLimit.Width;

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

        if (Work) return;

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

    public void SetAttack()
    {
        _Bag.AddItem(SpriteType.Wood, 999);
        if (Climb || Work || Attribute.AttackCooldown > 0) return;
        if (!EnabledAction()) return;

        if (classType == ClassType.Warrior || classType == ClassType.Worker) spriteFXs.Add(new SpriteFX(this, GetSide(), SpriteType.SwordEffect));
        if (classType == ClassType.Archer) spriteFXs.Add(new SpriteFX(this, GetSide(), SpriteType.ArrowEffect));

        var spFX = spriteFXs[spriteFXs.Count() - 1];
        spFX.InitialPosition();

        if (spFX.spriteType == SpriteType.ArrowEffect && HoldUp)
        {
            spFX.SetFallingSpeed(-1.5f);
        }
        else if (spFX.spriteType == SpriteType.ArrowEffect && HoldDown)
        {
            spFX.SetFallingSpeed(1.2f);
        }
        else
        {
            spFX.SetFallingSpeed(-1);
        }

        if (Combo)
        {
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
        if (!EnabledAction()) return;

        if (Work) return;
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

    protected void UpdateCooldown()
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

        if ((Owner.spriteType == SpriteType.Player1 || Owner.spriteType == SpriteType.Player2)
            && !Owner.Combo && !spriteFX.GetCombo() && Owner.classType != ClassType.Worker)
        {
            Owner.Attribute.AttackCooldown = Owner.Attribute.AttackCooldown / 3;
            Owner.Attribute.ComboTimeLimit = Owner.Attribute.BaseComboTimeLimit;
            Owner.Combo = true;
        }

        if (Owner.spriteType == SpriteType.Bot && !Owner.Combo && !spriteFX.GetCombo() && Owner.classType != ClassType.Worker)
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
        if (Attribute.Knockback > 0 && Work) Work = false;
        if (Attribute.Knockback > 0 && Walk) Walk = false;

        if (spriteFX.spriteType == SpriteType.ArrowEffect) spriteFX.Active = false;
    }

    public void UpdateSpriteFXDamage(List<Sprite> player_targets, List<SpriteBot> bot_targets, List<SpriteObject> object_targets)
    {
        player_targets = player_targets.Where(player => player.Team != Team && !player.IsDead()).ToList();
        bot_targets = bot_targets.Where(bot => bot.Team != Team && !bot.IsDead()).ToList();
        object_targets = object_targets.Where(obj => obj.Owner != null && obj.Owner.Team != Team).ToList();

        foreach (var damage in spriteFXs)
        {
            foreach (var player in player_targets)
            {
                if (!damage.IsDead()) damage.Damage(player);
            }
            foreach (var bot in bot_targets)
            {
                if (!damage.IsDead()) damage.Damage(bot);
            }
            foreach (var _object in object_targets)
            {
                if (!damage.IsDead()) damage.DamageObject(_object);
            }
        }
    }

    public Side GetSide()
    {
        if (Direction == Enums.Direction.StandRight
            || Direction == Enums.Direction.WalkRight
            || Direction == Enums.Direction.AttackRight
            || Direction == Enums.Direction.DeadRight
            || Direction == Enums.Direction.StuntRight
            || Direction == Enums.Direction.WorkRight
        )
        {
            return Side.Right;
        }

        if (Direction == Enums.Direction.StandLeft
            || Direction == Enums.Direction.WalkLeft
            || Direction == Enums.Direction.AttackLeft
            || Direction == Enums.Direction.DeadLeft
            || Direction == Enums.Direction.StuntLeft
            || Direction == Enums.Direction.WorkLeft
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
        if (Attribute.HP <= 0)
        {
            Dead();
            return true;
        }
        return false;
    }

    private void Dead()
    {
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
        Work = false;
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

    public void SetObject(SpriteType spriteType)
    {
        spriteObjects.Add(new SpriteObject(this, GetSide(), spriteType, new Vector2(0, 0)));
    }

    public void SetInteractionObjects(List<SpriteObject> objects)
    {
        //_Bag.AddItem(SpriteType.Wood, 9999);
        if (!EnabledAction()) return;

        //Item collect only        
        if (HoldDown)
        {
            var inner_objects = objects.Where(obj => 
                obj.spriteType == SpriteType.Wood || obj.spriteType == SpriteType.Seed ||
                obj.spriteType == SpriteType.Stone || obj.spriteType == SpriteType.Iron || obj.spriteType == SpriteType.Vine
            ).ToList();
            var _obj = GetInteractObject(inner_objects);

            if (_obj == null) return;

            switch (_obj.spriteType)
            {
                case SpriteType.Wood:
                    _obj.Active = false;
                    _Bag.AddItem(SpriteType.Wood, 5);
                    Icons.Add(new IconDisplay(SpriteType.Wood, 5));
                    break;

                case SpriteType.Seed:
                    _obj.Active = false;
                    _Bag.AddItem(SpriteType.Seed, 1);
                    Icons.Add(new IconDisplay(SpriteType.Seed, 1));
                    break;

                case SpriteType.Stone:
                    _obj.Active = false;
                    _Bag.AddItem(SpriteType.Stone, 3);
                    Icons.Add(new IconDisplay(SpriteType.Stone, 3));
                    break;

                case SpriteType.Iron:
                    _obj.Active = false;
                    _Bag.AddItem(SpriteType.Iron, 1);
                    Icons.Add(new IconDisplay(SpriteType.Iron, 1));
                    break;

                case SpriteType.Vine:
                    _obj.Active = false;
                    _Bag.AddItem(SpriteType.Vine, 3);
                    Icons.Add(new IconDisplay(SpriteType.Vine, 3));
                    break;

            }
            return;
        }

        WorkingID = 0;
        var _objects = objects.Where(obj =>
                obj.spriteType != SpriteType.Wood && obj.spriteType != SpriteType.Seed &&
                obj.spriteType != SpriteType.Stone && obj.spriteType != SpriteType.Iron && obj.spriteType != SpriteType.GrowingTree
        ).ToList();
        var _object = GetInteractObject(_objects);

        if (_object != null)
        {
            if (Work)
            {
                Work = false;
                SetSide(GetSide());
                return;
            }

            switch (_object.spriteType)
            {
                case SpriteType.Tree01:
                case SpriteType.ArcherTowerBuild:
                case SpriteType.Resource:
                case SpriteType.Digging:

                    WorkingID = _object.ID;
                    Work = true;
                    Walk = false;

                    if (GetSide() == Side.Right)
                    {
                        Position = new Vector2((_object.Position.X + _object.GetSize().X / 2) - Size.X, Position.Y);
                    }
                    if (GetSide() == Side.Left)
                    {
                        Position = new Vector2(_object.Position.X + _object.GetSize().X / 2, Position.Y);
                    }

                    break;
            }

            if (HoldUp) return;
            if (Climb)
            {
                Climb = false;
                SetRandomSide();
                return;
            }

            if (_object.spriteType == SpriteType.ArcherTower)
            {
                if (_object.Position.Y == GroundLevel) return;
                Climb = true;
                Walk = false;
                Ground = false;
                Position = new Vector2(_object.Position.X + _object.GetSize().X / 2 - Size.X / 2, Position.Y);
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

    private bool PlantSeed()
    {
        if (_Bag.UseItem(SpriteType.Seed, 1))
        {
            objectsBuild.Add(new SpriteObject(null, Side.None, SpriteType.GrowingTree, new Vector2((Position.X + (GetSize().X / 2)), Position.Y)));
            return true;
        }
        return false;
    }

    private bool BuildArcherTower()
    {
        if (_Bag.UseItem(SpriteType.Wood, 12))
        {
            objectsBuild.Add(new SpriteObject(this, Side.None, SpriteType.ArcherTowerBuild, new Vector2((Position.X + (GetSize().X / 2)), Position.Y)));
            return true;
        }
        return false;
    }

    private bool Dig()
    {
        if (_Bag.UseItem(SpriteType.Wood, 8))
        {
            objectsBuild.Add(new SpriteObject(this, Side.None, SpriteType.Digging, new Vector2((Position.X + (GetSize().X / 2)), Position.Y)));
            return true;
        }
        return false;
    }

    public void NextAction()
    {
        if (SelectedAction == Enums.Action.Dig)
        {
            SelectedAction = Enums.Action.None;
            IconDisplay = new IconDisplay(SelectedAction);
            return;
        }
        SelectedAction++;
        IconDisplay = new IconDisplay(SelectedAction);
    }

    public void PreviousAction()
    {
        if (SelectedAction == Enums.Action.None)
        {
            SelectedAction = Enums.Action.Dig;
            IconDisplay = new IconDisplay(SelectedAction);
            return;
        }
        SelectedAction--;
        IconDisplay = new IconDisplay(SelectedAction);
    }

    public void ActionExecute()
    {
        switch (SelectedAction)
        {
            case Enums.Action.BuildArcherTower:
                BuildArcherTower();
                break;

            case Enums.Action.PlantSeed:
                PlantSeed();
                break;

            case Enums.Action.Dig:
                Dig();
                break;
        }
    }

    public List<SpriteObject> GetObjectsBuild()
    {
        var temp_objects = objectsBuild;
        objectsBuild = new List<SpriteObject>();
        return temp_objects;
    }

    public void UpdateInteraction(List<SpriteObject> spriteObjects)
    {
        var inner_spriteObjects = spriteObjects.Where(_spriteobject => _spriteobject.Active && _spriteobject.ID == WorkingID).ToList();

        var _object = GetInteractObject(inner_spriteObjects);

        if (_object != null && Work && !_object.IsTransformed())
        {
            switch (_object.spriteType)
            {
                case SpriteType.Tree01:
                case SpriteType.Resource:
                    _object.UnbuildObject();
                    break;

                case SpriteType.ArcherTowerBuild:
                case SpriteType.Digging:
                    _object.BuildObject();
                    break;

                case SpriteType.ArcherTower:
                    _object.UnbuildObject();
                    break;
            }
        }
        else
        {
            Work = false;
            WorkingID = 0;
            if (GetSide() == Side.Right)
            {
                Direction = Enums.Direction.StandRight;
            }
            else if (GetSide() == Side.Left)
            {
                Direction = Enums.Direction.StandLeft;
            }
        }

        inner_spriteObjects = spriteObjects.Where(_spriteobject => _spriteobject.Active).ToList();
        _object = GetInteractObject(inner_spriteObjects);

        if (Climb && _object == null)
        {
            Climb = false;
            SetRandomSide();
        }

        if (_object == null || _object.AttributeObject.Build < _object.AttributeObject.MaxBuild)
        {
            GroundLevel = Globals.GroundLevel;
        }

        var bottom = (int)Position.Y + Size.Y;
        if (_object != null && !Ground && bottom < _object.Position.Y * 1.01)
        {
            if (_object.spriteType == SpriteType.ArcherTower)
            {
                GroundLevel = (int)(_object.Position.Y);
                Ground = true;
            }
        }

        //Needs buff list
        if (_object != null && (int)(_object.Position.Y) == GroundLevel)
        {
            //Climb = false;
            Attribute.BuffAttack = 5;
            Attribute.BuffKnockback = 2;
        }
        else
        {
            Attribute.BuffAttack = 0;
            Attribute.BuffKnockback = 0;
        }

    }

    private SpriteObject GetInteractObject(List<Sprite> players, List<SpriteBot> bots)
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

    private SpriteObject GetInteractObject(List<SpriteObject> objects)
    {
        Collision collision = new Collision();

        SpriteObject _object = null;
        objects = objects.Where(_spriteobject => !_spriteobject.IsDead()).ToList();
        foreach (var local_object in objects)
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

    protected Rectangle GetRectangle()
    {
        var AdjustSizX = (int)(Size.X * 0.5);
        var AdjustPosX = (int)(Position.X + ((Size.X - AdjustSizX)) / 2);

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

    private bool EnabledAction()
    {
        if (IsDead() || Attribute.StuntTime > 0) return false;
        return true;
    }

    public int GetBagItem(SpriteType spriteType)
    {
        return _Bag.GetItemQuantity(spriteType);
    }

    public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics)
    {
        //if (ID == 0) return;

        _anims.Draw(Position, 0.9f, deadAlpha);

        foreach (var attack in spriteFXs)
        {
            attack.Draw(spriteBatch, font, graphics, 1);
        }

        if (ID == 01 || ID == 02) spriteBatch.DrawString(font, "*", new Vector2(Position.X + 18, Position.Y - 20), Color.Red, 0f, Vector2.One, 1f, SpriteEffects.None, 1);

        if (IsDead() || ID == 0) return;

        //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "HP:" + Attribute.HP.ToString(), new Vector2(Position.X - 12, Position.Y-2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        Icons = Icons.Where(icon => icon.spriteType != SpriteType.None && icon.Time > 0).ToList();
        foreach (var icon in Icons)
        {
            spriteBatch.DrawString(font, icon.spriteType.ToString() + " +" + icon.Quantity, new Vector2(Position.X - 14, Position.Y - icon.PositionY), Color.Black * icon.Alpha, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, icon.spriteType.ToString() + " +" + icon.Quantity, new Vector2(Position.X - 14, Position.Y - 2 - icon.PositionY), Color.White * icon.Alpha, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

            icon.Alpha -= 0.5f * Globals.ElapsedSeconds;
            icon.Time -= 1 * Globals.ElapsedSeconds;
            icon.PositionY += 30 * Globals.ElapsedSeconds;
        }

        if (IconDisplay != null && (spriteType == SpriteType.Player1 || spriteType == SpriteType.Player2))
        {
            if (IconDisplay.spriteType == SpriteType.None && IconDisplay.Time > 0)
            {
                spriteBatch.DrawString(font, IconDisplay.action.ToString(), new Vector2(Position.X - 12, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, IconDisplay.action.ToString(), new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
            }

            IconDisplay.Time -= 1 * Globals.ElapsedSeconds;
        }

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
}
