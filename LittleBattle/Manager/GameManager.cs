using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Windows.Media.Capture.Core;
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
    private BotManager botManager;
    private Sprite Cameraman;
    public static KeyMappingsManager keyMappings;

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        this.graphics = graphics;
        Globals.Size = new Size(1920, 1080);
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
        resolution.SetFullScreen();
        Globals.GroundLevel = Globals.Size.Height - 60;

        Cameraman = new SpriteCameraman(00, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Cameraman, Team.None, ClassType.None);
        Cameraman.CenterX_Adjust();

        players = new List<Sprite>
        {
            new Sprite(01, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Player1, Team.Team1, ClassType.Worker),
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

        keyMappings = new KeyMappingsManager();
        keyMappings.LoadKeyMappings();

        objects = new List<SpriteObject>
        {
            new SpriteObject(null, Side.None, SpriteType.Tree01, new Vector2(100, Globals.GroundLevel)),
            new SpriteObject(null, Side.None, SpriteType.Resource, new Vector2(50, Globals.GroundLevel)),
        };

        //KeyMappingsManager custom = new KeyMappingsManager();
        //custom.MoveLeft = Microsoft.Xna.Framework.Input.Keys.A;
        //keyMappings.SaveCustomConfig(custom);
        //keyMappings = custom;
    }

    public void Update()
    {

        Globals.GroundLevel = Globals.Size.Height - 62;
        //InputManager.UpdateResolution(resolution);
        //InputManager.DebugCommand(players, bots);
        Globals.CameraMovement = Cameraman.CameraDirectionSpeed();
        backgroundManager.Update();

        Cameraman.Update();
        botManager.UpdateCamerman(Cameraman, players);
        foreach (var player in players)
        {
            InputManager.Update(players, bots, objects, resolution, keyMappings);
            player.Update();
            player.UpdateSpriteFXDamage(players, bots, objects);
            player.UpdateInteraction(objects);

            var objBuild = player.GetObjectsBuild();
            foreach (var obj in objBuild)
            {
                objects.Add(obj);
            }
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
        }

        if (InputManager.CommandBot)
        {
            botManager.Update(bots, players, true);
            InputManager.CommandBot = false;
        }
        else
        {
            botManager.Update(bots, players);
        }

        objects = objects.Where(_object => _object.Active).ToList();
        var new_objects = new List<SpriteObject>();
        foreach (var _object in objects)
        {
            bool putNewObject = _object.PutNewObject();
            Vector2 pos = new Vector2();
            if (putNewObject)
            {
                var pX = (_object.Position.X + (_object.GetSize().X / 2));
                var pY = _object.Position.Y;
                pos = new Vector2(pX, pY);
            }

            if (putNewObject && _object.spriteType == SpriteType.Tree01)
            {
                new_objects.Add(new SpriteObject(null, Side.None, SpriteType.Wood, pos + RandomPosition()));
                new_objects.Add(new SpriteObject(null, Side.None, SpriteType.Seed, pos + RandomPosition()));
                new_objects.Add(new SpriteObject(null, Side.None, SpriteType.Vine, pos + RandomPosition()));
            }
            if (putNewObject && _object.spriteType == SpriteType.Resource)
            {
                new_objects.Add(new SpriteObject(null, Side.None, SpriteType.Stone, pos + RandomPosition()));
                new_objects.Add(new SpriteObject(null, Side.None, SpriteType.Iron, pos + RandomPosition()));
            }
        }

        foreach (var _object in new_objects)
        {
            objects.Add(_object);
        }

        foreach (var _object in objects)
        {
            _object.Update();
        }
    }

    private Vector2 RandomPosition()
    {
        int randomValX, randomValY;
        Random random = new Random();
        randomValX = random.Next(120) * 1 - 60;
        randomValY = random.Next(60) * 1 - 30;
        return new Vector2(randomValX, randomValY);
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
        foreach (var obj in objects_layer0)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }

        //foreach (var player in players)
        //{
        //    player.DrawObjects(spriteBatch, font, graphics);
        //}

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
        if (Globals.Debug) debugManager.Draw(spriteBatch, font, players[0], players[0], bots);

        foreach (var obj in objects_layer1)
        {
            obj.Draw(spriteBatch, font, graphics, 0.1f);
        }

        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
