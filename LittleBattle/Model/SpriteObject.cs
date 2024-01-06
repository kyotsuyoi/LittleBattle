using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml.Controls;
using static LittleBattle.Classes.Enums;

public class SpriteObject
{
    public bool Active { get; set; }
    protected Texture2D texture;
    private readonly AnimationManager _anims = new AnimationManager();

    public Vector2 Position { get; set; }
    private Vector2 Size { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    private Side Side { get; set; }
    public SpriteType spriteType { get; }
    public float RelativeX { get; set; }
    public Sprite Owner { get; set; }
    public AttributeObject AttributeObject { get; set; }
    public int ID { get; }
    private float deadAlpha = 1f;

    public SpriteObject(Sprite Owner, Side side, SpriteType spriteType)
    {
        Active = true;
        ID = Globals.GetNewID();
        this.Owner = Owner;
        this.spriteType = spriteType;

        AttributeObject = new AttributeObject();

        Ground = false;
        FallingSpeed = -1;
        Side = side;

        SetTexture();

        InitialPosition();      
    }

    private void SetTexture()
    {
        int framesX = 4;
        int framesY = 1;

        if (spriteType == SpriteType.ArcherTower) { 
            if (Owner.Team == Team.Team1) texture = Globals.Content.Load<Texture2D>("Sprites/ArcherTower01_x3");
            if (Owner.Team == Team.Team2) texture = Globals.Content.Load<Texture2D>("Sprites/ArcherTower02_x3");
        }

        _anims.AddAnimation(Direction.StandRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 1, false, false));
        _anims.AddAnimation(Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 1, true, false));
        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
    }

    public void Update()
    {
        AnimationResolve();
        FallingResolve();

        Position += new Vector2(Globals.CameraMovement,0);
        RelativeX = Position.X - Globals.GroundX;

        if (IsDead())
        {
            _anims.Update(Direction.StandRight, false, -1);
        }
        else
        {
            _anims.Update(Direction.StandRight, false, 0);
        }
    }

    private void AnimationResolve()
    {
        if (IsDead())
        {
            deadAlpha -= 0.1f * Globals.ElapsedSeconds;
            if (deadAlpha <= 0) Active = false;
            return;
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
            //if(spriteType == SpriteType.ArrowEffect) Active = false;
        }
        else
        {
            Ground = false;
        }
    }

    private void InitialPosition()
    {
        var oCenterX = Owner.Position.X + Owner.GetSize().X /2;
        var oCenterY = Owner.Position.Y + Owner.GetSize().Y /2;
        var centerX = Size.X / 2;
        var centerY = Size.Y / 2;

        Position = new Vector2(oCenterX - centerX, oCenterY - centerY);

        if (Owner.GetSide() == Side.Right) Position += new Vector2(25, 0);
        if (Owner.GetSide() == Side.Left) Position -= new Vector2(25, 0);

        GroundLevel = Globals.GroundLevel - Size.Y;
    }

    public void TakeDamage(SpriteFX spriteFX)
    {
        if (!spriteFX.Active) return;
        var Owner = spriteFX.Owner;
        var res = (Owner.Attribute.Attack + spriteFX.AttributeFX.Damage + Owner.Attribute.BuffAttack);
        if (res < 1) res = 1;

        AttributeObject.HP -= res;
        if (IsDead()) return;
        spriteFX.Active = false;
    }
    
    public bool IsDead()
    {
        if (AttributeObject.HP <= 0)
        {
            Dead();
            return true;
        }
        return false;
    }

    private void Dead()
    {
        AttributeObject.HP = 0;
        //Active = false;
    }

    public void CenterX_Adjust()
    {
        Position = new Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
    }

    public void SetToGroundLevel(float positionX)
    {
        Position = new Vector2(Position.X, GroundLevel);
    }

    public Enums.Side GetSide()
    {
        return Side;
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

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, float layerDepth)
    {
        Texture2D _texture;
        if (Globals.Debug && Globals.DebugArea)
        {
            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Red });
            spriteBatch.Draw(_texture, GetRectangle(), Color.Red * 0.4f);
        }

        _anims.Draw(Position, layerDepth, deadAlpha);

        //spriteBatch.DrawString(font, "frame:" + Globals.SpriteFrame.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "frame:" + Globals.SpriteFrame.ToString(), new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        var _hp_percent = AttributeObject.HP * 100 / AttributeObject.BaseHP;
        if (_hp_percent >= 100 || _hp_percent <= 0) return;
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

        if (Globals.Debug && Globals.DebugArea)
        {
            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Blue });
            spriteBatch.Draw(_texture, GetRectangle(), Color.Blue * 0.4f);
        }
    }
}
