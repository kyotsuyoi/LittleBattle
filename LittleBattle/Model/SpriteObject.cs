using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using static LittleBattle.Classes.Enums;

public class SpriteObject
{
    public bool Active { get; set; }
    private Texture2D texture;
    private AnimationManager _anims = new AnimationManager();
    public Vector2 Position { get; set; }
    private Vector2 Size { get; set; }
    public bool Ground { get; set; }
    public float GroundLevel { get; set; }
    public float FallingSpeed { get; set; }
    private Side Side { get; set; }
    public SpriteType spriteType { get; set; }
    public float RelativeX { get; set; }
    public Sprite Owner { get; set; }
    public AttributeObject AttributeObject { get; set; }
    public int ID { get; }

    private float deadAlpha = 1f;

    private bool putNewObject = false;
    private bool dropNewObject = false;

    private bool transformed = false;

    public int layer = 0;

    public bool InteractionPointer { get; set; }

    private Texture2D debugArea;
    private Texture2D hpBarBackground;
    private Texture2D hpBarForeground;

    Collision collision;
    List<SpriteObjectItem> returnObjects;

    public SpriteObject(Sprite Owner, Side side, SpriteType spriteType, Vector2 initialPosition, GraphicsDeviceManager graphics)
    {
        Active = true;
        ID = Globals.GetNewID();
        this.Owner = Owner;
        this.spriteType = spriteType;

        Ground = false;
        FallingSpeed = -1;
        Side = side;

        SetTexture();
        InitialPosition(initialPosition);

        debugArea = new Texture2D(graphics.GraphicsDevice, 1, 1);
        hpBarBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
        hpBarForeground = new Texture2D(graphics.GraphicsDevice, 1, 1);
        debugArea.SetData(new Color[] { Color.Blue });
        hpBarBackground.SetData(new Color[] { Color.Black });
        hpBarForeground.SetData(new Color[] { Color.GreenYellow });

        collision = new Collision();
        returnObjects = new List<SpriteObjectItem>();
    }

    private void SetTexture()
    {
        int framesX = 4;
        int framesY = 1;

        if (spriteType == SpriteType.ArcherTower) 
        { 
            if (Owner.Team == Team.Team1) texture = Globals.Content.Load<Texture2D>("Sprite_x3/ArcherTower01");
            if (Owner.Team == Team.Team2) texture = Globals.Content.Load<Texture2D>("Sprite_x3/ArcherTower02");
        }

        if(spriteType == SpriteType.Tree01)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree01");
            layer = 0;
        }

        if (spriteType == SpriteType.Tree02)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree02");
            layer = 0;
        }

        if (spriteType == SpriteType.Wood)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Wood");
            layer = 1;
        }

        if (spriteType == SpriteType.Seed01)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Seed01");
            layer = 1;
        }

        if (spriteType == SpriteType.Seed02)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Seed02");
            layer = 1;
        }

        if (spriteType == SpriteType.Tree01Growing)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree01Growing");
            layer = 0;
        }


        if (spriteType == SpriteType.Tree02Growing)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree02Growing");
            layer = 0;
        }

        if (spriteType == SpriteType.ArcherTowerBuilding)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/ArcherTowerBuild");
            layer = 0;
        }

        if (spriteType == SpriteType.Stone)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Stone");
            layer = 1;
        }

        if (spriteType == SpriteType.Iron)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Iron");
            layer = 1;
        }

        if (spriteType == SpriteType.ResourceStone)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/ResourceStone");
            layer = 0;
        }

        if (spriteType == SpriteType.ResourceIron)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/ResourceIron");
            layer = 0;
        }

        if (spriteType == SpriteType.Digging)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Digging");
            layer = 0;
        }

        if (spriteType == SpriteType.Vine)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Vine");
            layer = 1;
        }

        if (spriteType == SpriteType.ToolBag)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/ToolBag");
            layer = 1;
        }

        if (spriteType == SpriteType.WorkStationBuilding)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/WorkStationBuilding");
            layer = 0;
        }

        if (spriteType == SpriteType.WorkStation)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/WorkStation");
            layer = 0;
        }

        if (spriteType == SpriteType.ReferencePointBuilding)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/ReferencePointBuilding");
            layer = 0;
        }

        if (spriteType == SpriteType.ReferencePoint)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/ReferencePoint");
            layer = 0;
        }

        if (spriteType == SpriteType.Fruit)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/Fruit");
            layer = 1;
        }

        if (spriteType == SpriteType.SetBagWorker)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/SetBagWorker");
            layer = 1;
        }

        if (spriteType == SpriteType.Tree01MidLife)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree01MidLife");
            layer = 0;
        }

        if (spriteType == SpriteType.Tree02MidLife)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree02MidLife");
            layer = 0;
        }

        if (spriteType == SpriteType.Tree01EndLife)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree01EndLife");
            layer = 0;
        }

        if (spriteType == SpriteType.Tree02EndLife)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/Tree02EndLife");
            layer = 0;
        }

        if (spriteType == SpriteType.TreeDried)
        {
            texture = Globals.Content.Load<Texture2D>("Sprite_x3/TreeDried");
            layer = 0;
        }

        if (spriteType == SpriteType.FruitRotten)
        {
            framesX = 1;
            texture = Globals.Content.Load<Texture2D>("Sprite/FruitRotten");
            layer = 1;
        }

        if (spriteType == SpriteType.None) return;

        _anims.AddAnimation(Direction.StandRight, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 1, false, false));
        _anims.AddAnimation(Direction.StandLeft, new Animation(texture, framesX, framesY, 0, 3, 0.2f, 1, true, false));
        Size = new Vector2(texture.Width / framesX, texture.Height / framesY);

        AttributeObject = new AttributeObject(spriteType);
    }

    public void Update(List<SpriteObject> objects)
    {        
        AnimationResolve();
        FallingResolve();

        Position += new Vector2(Globals.CameraMovement,0);
        RelativeX = Position.X - Globals.GroundX;

        var direction = Direction.None;
        if (Side == Side.Right) direction = Direction.StandRight;
        if (Side == Side.Left) direction = Direction.StandLeft;

        if (IsDead())
        {
            _anims.Update(direction, false);
        }
        else
        {
            _anims.Update(direction, false, 0);
        }

        UpdateBuild(objects);
    }

    private void UpdateBuild(List<SpriteObject> objects)
    {
        if (spriteType == SpriteType.Tree01Growing || spriteType == SpriteType.Tree02Growing || spriteType == SpriteType.ArcherTowerBuilding || spriteType == SpriteType.Digging || 
            spriteType == SpriteType.WorkStationBuilding || spriteType == SpriteType.ReferencePointBuilding)
        {
            if (AttributeObject.Build > AttributeObject.MaxBuild)
            {
                AttributeObject.Build = AttributeObject.MaxBuild;
            }
            double percent = (float)AttributeObject.Build / AttributeObject.MaxBuild * 100;

            if (percent >= 100)
            {
                transformed = true;
                switch (spriteType)
                {
                    case SpriteType.Tree01Growing:
                        spriteType = SpriteType.Tree01;
                        transformed = false;
                        break;

                    case SpriteType.Tree02Growing:
                        spriteType = SpriteType.Tree02;
                        transformed = false;
                        AttributeObject.Build = 0;
                        break;

                    case SpriteType.ArcherTowerBuilding:
                        spriteType = SpriteType.ArcherTower;
                        break;

                    case SpriteType.Digging:
                        spriteType = SpriteType.ResourceStone;
                        break;

                    case SpriteType.WorkStationBuilding:
                        spriteType = SpriteType.WorkStation;
                        break;

                    case SpriteType.ReferencePointBuilding:
                        spriteType = SpriteType.ReferencePoint;
                        break;
                }

                _anims.Update(Direction.StandRight, false, 0);
                _anims = new AnimationManager();
                SetTexture();

                if (spriteType == SpriteType.ResourceStone || spriteType == SpriteType.ResourceIron)
                {
                    InitialPosition(Position);
                }
            }
            else if (percent >= 75)
            {
                _anims.Update(Direction.StandRight, false, 3);
            }
            else if (percent >= 50)
            {
                _anims.Update(Direction.StandRight, false, 2);
            }
            else if (percent >= 25)
            {
                _anims.Update(Direction.StandRight, false, 1);
            }

            if (spriteType == SpriteType.Tree01Growing || spriteType == SpriteType.Tree02Growing)
            {
                if (IsTreeCollision(objects) && percent >= 25) return;
                BuildObject();
            }
        }

        //Fruit 
        //Counting LifeTime Cycles (Needs Adjust)
        if(spriteType == SpriteType.Tree02 || spriteType == SpriteType.Tree02MidLife || spriteType == SpriteType.Tree02EndLife ||
           spriteType == SpriteType.Tree01 || spriteType == SpriteType.Tree01MidLife || spriteType == SpriteType.Tree01EndLife ||
           spriteType == SpriteType.Fruit  || spriteType == SpriteType.FruitRotten   || spriteType == SpriteType.TreeDried)
        {
            BuildNewObject();
            AttributeObject.MaxBuild = 5;
            double percent = (float)AttributeObject.BuildNew / AttributeObject.MaxBuild * 100;
            if (percent >= 100)
            {
                dropNewObject = true;
            }
        }
    }

    private bool IsTreeCollision(List<SpriteObject> objects)
    {
        Collision collision = new Collision();

        var Trees = objects.Where(_spriteobject => !_spriteobject.IsDead() && 
        (_spriteobject.spriteType == SpriteType.Tree02 || _spriteobject.spriteType == SpriteType.Tree02MidLife || _spriteobject.spriteType == SpriteType.Tree02EndLife ||
        _spriteobject.spriteType == SpriteType.Tree01 || _spriteobject.spriteType == SpriteType.Tree01MidLife || _spriteobject.spriteType == SpriteType.Tree01EndLife ||
        _spriteobject.spriteType == SpriteType.TreeDried) &&
        ID != _spriteobject.ID).ToList();

        foreach (var Tree in Trees)
        {
            var PosB = new Point((int)Tree.Position.X, (int)Tree.Position.Y);
            var SizB = new Point((int)Tree.GetSize().X, (int)Tree.GetSize().Y);
            var RectB = new Rectangle(PosB, SizB);
            var isCollide = collision.IsCollide(GetRectangle(), RectB);

            if (isCollide)
            {
                return true;
            }
        }

        return false;
    }

    public List<SpriteObjectItem> FruitCollision(List<SpriteObjectItem> objects)
    {
        objects = objects.Where(_spriteobject => !_spriteobject.IsDead() && (_spriteobject.spriteType == SpriteType.FruitRotten) && ID != _spriteobject.ID).ToList();
        foreach (var local_object in objects)
        {
            var PosB = new Point((int)local_object.Position.X, (int)local_object.Position.Y);
            var SizB = new Point((int)local_object.GetSize().X, (int)local_object.GetSize().Y);
            var RectB = new Rectangle(PosB, SizB);
            var isCollide = collision.IsCollide(GetRectangle(), RectB);

            if (isCollide)
            {
                returnObjects.Add(local_object);
            }
        }

        return returnObjects;
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

    private void InitialPosition(Vector2 center_position)
    {
        if (Owner == null)
        {
            GroundLevel = Globals.GroundLevel - Size.Y;
            Position =  new Vector2(center_position.X - GetSize().X /2, center_position.Y);
            return;
        }
        
        Position = center_position;
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

    public void UnbuildObject()
    {
        AttributeObject.HP -= Globals.ElapsedSeconds*10;
    }

    public void BuildObject()
    {
        AttributeObject.Build += Globals.ElapsedSeconds*10;
    }

    private void BuildNewObject()
    {
        AttributeObject.BuildNew += Globals.ElapsedSeconds;
    }

    public bool Building()
    {
        if (AttributeObject.Build >= AttributeObject.MaxBuild) return false;
        return true;
    }

    public bool PutNewObject()
    {
        if (IsDead() && !putNewObject)
        {
            putNewObject = true;
            return true;
        }
        return false;
    }

    public bool DropNewObject()
    {

        if (IsDead()) return false;
        if (dropNewObject)
        {
            dropNewObject = false;
            AttributeObject.BuildNew = 0;
            AttributeObject.BuildNewCounter++;

            if(spriteType == SpriteType.Tree02 && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;

                spriteType = SpriteType.Tree02MidLife;
                //if (commom.PercentualCalc(50))
                //{
                //    spriteType = SpriteType.Tree01MidLife;
                //}

                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.Tree02MidLife && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.Tree02EndLife;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.Tree02EndLife && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.TreeDried;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.Tree01 && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.Tree01MidLife;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.Tree01MidLife && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.Tree01EndLife;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.Tree01EndLife && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.TreeDried;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.TreeDried && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.HP = 0;
            }

            if (spriteType == SpriteType.Fruit && AttributeObject.BuildNewCounter >= 5)
            {
                AttributeObject.BuildNewCounter = 0;
                spriteType = SpriteType.FruitRotten;
                _anims = new AnimationManager();
                SetTexture();
            }

            if (spriteType == SpriteType.FruitRotten && AttributeObject.BuildNewCounter >= 5)
            {
                Active = false;
            }

            return true;
        }
        return false;
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

    public void SetToGroundLevel()
    {
        Position = new Vector2(Position.X, Globals.GroundX - Size.Y);
    }

    public bool IsTransformed()
    {
        if(transformed)
        {
            transformed = false;
            return true;
        }
        return false;
    }

    public Enums.Side GetSide()
    {
        return Side;
    }

    public void SetSideBuild(Enums.Side side)
    {
        Side = side;
        var direction = Direction.None;
        if (Side == Side.Right) direction = Direction.StandRight;
        if (Side == Side.Left) direction = Direction.StandLeft;
        _anims.Update(direction, false, 0);
    }

    public Rectangle GetRectangle()
    {
        var Pos = new Point((int)Position.X, (int)Position.Y);
        var Siz = new Point((int)Size.X, (int)Size.Y);

        return new Rectangle(Pos, Siz);
    }

    public Vector2 GetSize()
    {
        return Size;
    }

    public void SetAlpha(float alpha)
    {
        deadAlpha = alpha;
    }

    private void DisplayHP(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        var barSize = 40;
        var _hp_percent = AttributeObject.HP * 100 / AttributeObject.BaseHP;
        if (_hp_percent >= 100 || _hp_percent <= 0) return;
        var hp_val = barSize * _hp_percent / 100;

        // Barra de fundo
        spriteBatch.Draw(hpBarBackground, new Rectangle((int)(Position.X + Size.X / 2 - barSize / 2), (int)(Position.Y - 10), barSize, 4), Color.Black * 0.6f);

        // Cor da barra de vida
        var _color = Color.GreenYellow;
        if (_hp_percent < 75) _color = Color.Yellow;
        if (_hp_percent < 50) _color = Color.Orange;
        if (_hp_percent < 25) _color = Color.Red;

        // Barra de vida (usa a textura branca e aplica a cor desejada)
        spriteBatch.Draw(hpBarForeground, new Rectangle((int)(Position.X + Size.X / 2 - barSize / 2), (int)(Position.Y - 10), (int)hp_val, 4), _color * 0.6f);
    }

    private void DisplayPointer(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!InteractionPointer || IsDead()) return;

        int RefPX = 7;
        int RefPY = 12;
        if(AttributeObject.HP < AttributeObject.BaseHP)
        {
            RefPY += 16;
        }
        //spriteBatch.DrawString(font, "l", new Vector2(Position.X - 08 + Size.X / 2 + RefPX+2, Position.Y - 10 - RefPY), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "l", new Vector2(Position.X - 10 + Size.X / 2 + RefPX+2, Position.Y - 12 - RefPY), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        spriteBatch.DrawString(font, "Y", new Vector2(Position.X - 10 + Size.X / 2 + RefPX, Position.Y - 00 - RefPY), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        spriteBatch.DrawString(font, "Y", new Vector2(Position.X - 12 + Size.X / 2 + RefPX, Position.Y - 02 - RefPY), Color.Yellow, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, float layerDepth)
    {
        if (Globals.Debug && Globals.DebugArea)
        {            
            spriteBatch.Draw(debugArea, GetRectangle(), Color.Red * 0.4f);
        }

        _anims.Draw(Position, layerDepth, deadAlpha);

        //spriteBatch.DrawString(font, "frame:" + Globals.SpriteFrame.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "frame:" + Globals.SpriteFrame.ToString(), new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        //float percent = (float)AttributeObject.Build / (float)AttributeObject.MaxBuild * 100;
        //spriteBatch.DrawString(font, "Build:" + percent, new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "Build:" + percent, new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        //float percent = (float)AttributeObject.BuildNew / (float)AttributeObject.MaxBuild * 100;
        //spriteBatch.DrawString(font, "Build:" + percent, new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "Build:" + percent, new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        //spriteBatch.DrawString(font, "HP:" + AttributeObject.HP, new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "HP:" + AttributeObject.HP, new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        //spriteBatch.DrawString(font, "C:" + AttributeObject.BuildNewCounter.ToString(), new Vector2(Position.X - 10, Position.Y), Color.Black, 0f, Vector2.One, 1f, SpriteEffects.None, 1);
        //spriteBatch.DrawString(font, "C:" + AttributeObject.BuildNewCounter.ToString(), new Vector2(Position.X - 12, Position.Y - 2), Color.White, 0f, Vector2.One, 1f, SpriteEffects.None, 0.9999f);

        DisplayHP(spriteBatch, graphics);
        DisplayPointer(spriteBatch, font);

        if (Globals.Debug && Globals.DebugArea)
        {
            spriteBatch.Draw(debugArea, GetRectangle(), Color.Blue * 0.4f);
        }
    }
}
