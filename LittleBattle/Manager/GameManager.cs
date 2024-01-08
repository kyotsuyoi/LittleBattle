using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            new Sprite(01, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Player1, Team.Team1, ClassType.Archer),
            new Sprite(02, new Vector2((Globals.Size.Width / 2), 504), SpriteType.Player2, Team.Team2, ClassType.Archer),
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
            InputManager.Update(players, bots, resolution, keyMappings);
            player.Update();
            player.UpdateSpriteFXDamage(players);
            player.UpdateSpriteFXDamage(bots);
            player.UpdateSpriteObjects();
            player.UpdateInteraction(players, bots);
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
            bot.UpdateSpriteFXDamage(players);
            bot.UpdateSpriteFXDamage(bots);
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

        foreach (var player in players)
        {
            player.DrawObjects(spriteBatch, font, graphics);
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
        if (Globals.Debug) debugManager.Draw(spriteBatch, font, players[0], players[0], bots);
        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
