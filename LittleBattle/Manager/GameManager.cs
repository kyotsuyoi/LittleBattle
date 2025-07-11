using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics; // To monitoring memory usage
using static LittleBattle.Classes.Enums;

public class GameManager
{
    private GraphicsDeviceManager graphics;
    private readonly List<Sprite> players;
    private readonly Canvas _canvas;
    private readonly Resolution resolution;
    private readonly BackgroundManager backgroundManager;
    private readonly SpriteFont font;
    private DebugManager debugManager;
    private List<SpriteBot> bots;
    private List<SpriteObject> objects;
    private List<SpriteObjectItem> objectItems;
    private BotManager botManager;
    private SpriteCameraman Cameraman;
    public static KeyMappingsManager keyMappings;
    private Common Common;

    private Vector2 pos;

    // Memory usage (bytes)
    private long memoryUsage;
    private double memoryTimer = 0;

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        this.graphics = graphics;
        Globals.Size = new Size(1920, 1080);
        //Globals.Size = new Size(3840, 2160);
        
        _canvas = new Canvas(graphics.GraphicsDevice, Globals.Size.Width, Globals.Size.Height);

        Globals.Debug = false;
        Globals.DebugArea = false;

        backgroundManager = new BackgroundManager();
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Mountain"), 0, 0.4f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Trees"), 0.1f, 0.6f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Fog"), 0.2f, 0.8f, true, 20f, 0.2f));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Ground"), 0.3f, 1f, false));

        resolution = new Resolution(game, graphics, _canvas);
        resolution.SetResolution(Globals.Size);
        //resolution.SetFullScreen();
        Globals.GroundLevel = Globals.Size.Height - 60;

        Cameraman = new SpriteCameraman(00, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Cameraman, Team.None, ClassType.None, graphics);
        Cameraman.CenterX_Adjust();

        players = new List<Sprite>
        {
            new Sprite(01, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Player1, Team.Team1, ClassType.Newbie, graphics),
            //new Sprite(02, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Player2, Team.Team2, ClassType.Archer),
        };
        foreach (var player in players)
        {
            player.CenterX_Adjust();
        }

        bots = new List<SpriteBot>();

        foreach (var bot in bots)
        {
            bot.CenterX_Adjust();
        }

        Globals.Gravity = 10;
        Globals.PositiveLimit = new Size(2000, 0);
        Globals.NegativeLimit = new Size(-2000, 0);

        font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
        debugManager = new DebugManager();
        botManager = new BotManager();
        Common = new Common();
        pos = new Vector2();

        keyMappings = new KeyMappingsManager();
        keyMappings.LoadKeyMappings();

        objects = new List<SpriteObject>
        {
            new SpriteObject(null, Side.Right, SpriteType.Tree01, new Vector2(100, Globals.GroundLevel), graphics),
            new SpriteObject(null, Side.Right, SpriteType.ResourceStone, new Vector2(50, Globals.GroundLevel), graphics),
            new SpriteObject(null, Side.Right, SpriteType.ResourceIron, new Vector2(150, Globals.GroundLevel), graphics),
        };

        objectItems = new List<SpriteObjectItem>
        {
            new SpriteObjectItem(null, Side.None, SpriteType.ToolBag, new Vector2(10, Globals.GroundLevel), 1, graphics),
            new SpriteObjectItem(null, Side.None, SpriteType.SetBagWorker, new Vector2(-10, Globals.GroundLevel), 1, graphics),
        };

        //KeyMappingsManager custom = new KeyMappingsManager();
        //custom.MoveLeft = Microsoft.Xna.Framework.Input.Keys.A;
        //keyMappings.SaveCustomConfig(custom);
        //keyMappings = custom;
    }

    public void Update()
    {
        Globals.GroundLevel = Globals.Size.Height - 62;
        Globals.CameraMovement = Cameraman.CameraDirectionSpeed();
        backgroundManager.Update();

        Cameraman.Update();
        botManager.UpdateCamerman(Cameraman, players);
        foreach (var player in players)
        {
            InputManager.Update(players, bots, objects, objectItems, Cameraman, resolution, _canvas, keyMappings, graphics);
            player.Update();
            player.UpdateSpriteFXDamage(players, bots, objects);
            player.UpdateInteraction(objects, objectItems);

            var objBuild = player.GetObjectsBuild();
            var objNewBuild = player.GetNewObjectsBuild();
            var objNewBots = player.GetNewBots();
            foreach (var obj in objBuild)
            {
                objects.Add(obj);
            }
            foreach (var obj in objNewBuild)
            {
                objectItems.Add(obj);
            }
            foreach (var obj in objNewBots)
            {
                bots.Add(obj);
            }
        }

        // Atualiza o timer e armazena o consumo de mem�ria a cada segundo
        memoryTimer += (double)Globals.ElapsedSeconds;
        if (memoryTimer >= 1.0)
        {
            memoryUsage = GC.GetTotalMemory(false); //Process.GetCurrentProcess().WorkingSet64;
            memoryTimer = 0;
        }

        //Debug Command
        if (InputManager.ClearBot)
        {
            bots = bots.Where(bot => !bot.IsDead()).ToList();
            InputManager.ClearBot = false;
        }

        bots = bots.Where(bot => bot.Active).ToList();
        foreach (var bot in bots)
        {
            bot.Update();
            bot.UpdateSpriteFXDamage(players, bots, objects);
            bot.UpdateInteraction(objects, objectItems);
        }

        if (InputManager.CommandBot)
        {
            botManager.Update(bots, players, objectItems, objects, true);
            InputManager.CommandBot = false;
        }
        else
        {
            botManager.Update(bots, players, objectItems, objects);
        }
        UpdateObjects();
    }

    private void UpdateObjects()
    {
        objects = objects.Where(_object => _object.Active).ToList();
        objectItems = objectItems.Where(_object => _object.Active).ToList();
        var new_objectItems = new List<SpriteObjectItem>();

        foreach (var _objectItem in objects)
        {
            bool putNewObject = _objectItem.PutNewObject();
            if (putNewObject)
            {
                var pX = (_objectItem.Position.X + (_objectItem.GetSize().X / 2));
                var pY = _objectItem.Position.Y;
                pos = new Vector2(pX, pY);
            }

            if (putNewObject)
            {
                new_objectItems = Common.RandomObjects(new_objectItems, _objectItem.spriteType, pos, graphics);
            }

            if (_objectItem.DropNewObject() && (_objectItem.spriteType == SpriteType.Tree02 || _objectItem.spriteType == SpriteType.Tree02MidLife))
            {
                var pX = (_objectItem.Position.X + (_objectItem.GetSize().X / 2));
                var pY = _objectItem.Position.Y;
                pos = new Vector2(pX, pY);
                new_objectItems = Common.RandomObjects(new_objectItems, SpriteType.Fruit, pos, graphics);
            }
        }

        foreach (var _object in new_objectItems)
        {
            objectItems.Add(_object);
        }

        var maxTrees = objects.Where(_obj => _obj.spriteType == SpriteType.Tree02Growing || _obj.spriteType == SpriteType.Tree02).ToList();
        var maxFruit = objects.Where(_obj => _obj.spriteType == SpriteType.Fruit || _obj.spriteType == SpriteType.FruitRotten).ToList();
        foreach (var _object in objectItems)
        {
            _object.Update(objects);           
            var rottenFruits = _object.FruitCollision(objectItems);
            if (_object.DropNewObject() && _object.spriteType == SpriteType.FruitRotten && Common.PercentualCalc(5 + (1 * rottenFruits.Count())))
            {
                //objects.Add(new SpriteObject(null, Side.None, SpriteType.Tree02Growing, _object.Position));

                if (maxTrees.Count <= 60 && maxFruit.Count <= 100) objects.Add(new SpriteObject(null, Side.Left, SpriteType.Tree02Growing, _object.Position, graphics));
                foreach (var rottenFruit in rottenFruits)
                {
                    rottenFruit.Active = false;
                }
            }
        }

        foreach (var _object in objects)
        {
            _object.Update(objects);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _canvas.Activate();

        spriteBatch.Begin();
        backgroundManager.Draw();
        if (InputManager.visibleCamerman && Globals.Debug) Cameraman.Draw(spriteBatch, font, graphics);

        var deadBots = bots.Where(bot => bot.IsDead()).ToList();
        var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
        var deadPlayers = players.Where(player => player.IsDead()).ToList();
        var alivePlayers = players.Where(player => !player.IsDead()).ToList();

        var objects_layer0 = objects.Where(objects => objects.layer == 0).ToList();
        var objects_layer1 = objects.Where(objects => objects.layer == 1).ToList();

        var objectItems_layer0 = objectItems.Where(objects => objects.layer == 0).ToList();
        var objectItems_layer1 = objectItems.Where(objects => objects.layer == 1).ToList();

        foreach (var obj in objects_layer0)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }
        foreach (var obj in objectItems_layer0)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }

        foreach (var bot in deadBots)
        {
            bot.Draw(spriteBatch, font, graphics);
        }
        foreach (var player in deadPlayers)
        {
            player.Draw(spriteBatch, font, graphics);
        }
        foreach (var bot in aliveBots)
        {
            bot.Draw(spriteBatch, font, graphics);
        }
        alivePlayers = alivePlayers.OrderByDescending(player => player.spriteType).ToList();
        foreach (var player in alivePlayers)
        {
            player.Draw(spriteBatch, font, graphics);
        }

        debugManager.Draw(spriteBatch, font, players[0], players[0], bots, _canvas, Cameraman, memoryUsage);

        foreach (var obj in objects_layer1)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }
        foreach (var obj in objectItems_layer1)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }

        spriteBatch.End();
        _canvas.Draw(spriteBatch);
    }

}
