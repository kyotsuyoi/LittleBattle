using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static LittleBattle.Classes.Enums;

public class SpriteObject
{
    protected Texture2D texture;
    private readonly AnimationManager _anims = new AnimationManager();

    public Vector2 Position { get; set; }
    public Vector2 Size { get; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    private Enums.Side Side { get; set; }
    public Enums.SpriteType spriteType { get; }
    public float RelativeX { get; set; }
    public bool Active { get; set; }
    public Sprite Owner { get; set; }
    public AttributeObject AttributeObject { get; set; }
    public int ID { get; }

    public SpriteObject(Sprite Owner, Enums.Side side, Enums.SpriteType spriteType, int framesX, int framesY)
    {
        ID = Globals.GetNewID();
        this.Owner = Owner;
        this.spriteType = spriteType;
        Active = true;

        AttributeObject = new AttributeObject();

        Ground = false;
        FallingSpeed = -1;
        this.Side = side;

        if (spriteType == SpriteType.ArcherTower) texture = Globals.Content.Load<Texture2D>("Sprites/ArcherTower");

        _anims.AddAnimation(Enums.Direction.StandRight, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, false, false));
        _anims.AddAnimation(Enums.Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 11, 0.01f, 1, true, false));

        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);
        InitialPosition();      
    }

    public void Update()
    {
        FallingResolve();

        Position += new Vector2(Globals.CameraMovement,0);
        RelativeX = Position.X - Globals.GroundX;
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

    private void InitialPosition()
    {
        var oCenterX = Owner.Position.X + Owner.Size.X /2;
        var oCenterY = Owner.Position.Y + Owner.Size.Y /2;
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
        Active = false;
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

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, float layerDepth)
    {
        Texture2D _texture;
        if (Globals.Debug && Globals.DebugArea)
        {
            _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.Red });
            spriteBatch.Draw(_texture, GetRectangle(), Color.Red * 0.4f);
        }

        _anims.Draw(Position, layerDepth);
        //spriteBatch.DrawString(font, "HP:" + AttributeObject.HP.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "HP:" + AttributeObject.HP.ToString(), new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        _texture = new Texture2D(graphics.GraphicsDevice, 1, 1);
        _texture.SetData(new Color[] { Color.Black });
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X, 4), Color.Black * 0.6f);

        var _hp_percent = AttributeObject.HP * 100 / AttributeObject.BaseHP;
        var hp_val = Size.X * _hp_percent / 100;

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
