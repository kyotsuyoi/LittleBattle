using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class GameManager
{
    private readonly List<Sprite> players;
    private readonly Canvas _canvas;
    private readonly Resolution resolution;
    private readonly BackgroundManager backgroundManager;
    private readonly SpriteFont font;
    private DebugManager debugManager;
    private List<Sprite> bots;
    private BotManager botManager;
    private Sprite Cameraman;
    public static KeyMappingsManager keyMappings;

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        Globals.Size = new Size(1920, 1080);
        _canvas = new Canvas(graphics.GraphicsDevice, Globals.Size.Width, Globals.Size.Height);
        backgroundManager = new BackgroundManager();
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Mountain"), 0.6f, 0.6f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Trees"), 0.8f, 0.8f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Ground"), 1f, 1f, false));

        resolution = new Resolution(game, graphics, _canvas);
        resolution.SetResolution(Globals.Size);
        resolution.SetFullScreen();

        Cameraman = new Sprite(00, new Vector2((Globals.Size.Width / 2), 504), Enums.SpriteType.Cameraman, 4, 4, Enums.Team.None, Enums.ClassType.None);
        Cameraman.CenterX_Adjust();
        Cameraman.SetToGroundLevel(0);

        players = new List<Sprite>
        {
            new Sprite(01, new Vector2((Globals.Size.Width / 2), 504), Enums.SpriteType.Player1, 4, 5, Enums.Team.Team1, Enums.ClassType.Archer),
            //new Sprite(02, new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.SpriteType.Player2, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
        };
        foreach (var player in players)
        {
            player.CenterX_Adjust();
            player.SetToGroundLevel(0);
        }

        bots = new List<Sprite>();

        foreach (var bot in bots)
        {
            bot.CenterX_Adjust();
            bot.SetToGroundLevel(0);
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
        InputManager.UpdateResolution(resolution);
        //Globals.CameraMovement = players[0].DirectionSpeed();
        Globals.CameraMovement = Cameraman.CameraDirectionSpeed();
        backgroundManager.Update();

        Cameraman.Update();
        botManager.UpdateCamerman(Cameraman, players);
        foreach (var player in players)
        {
            InputManager.Update(player, bots,keyMappings);
            player.Update();
            player.UpdateSpriteFXDamage(players);
            player.UpdateSpriteFXDamage(bots);
        }
        foreach (var bot in bots)
        {
            botManager.UpdateCamerman(Cameraman, players);
            bot.Update();
            bot.UpdateSpriteFXDamage(players);
            bot.UpdateSpriteFXDamage(bots);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _canvas.Activate();

        spriteBatch.Begin();
        backgroundManager.Draw();
        if (InputManager.visibleCamerman) Cameraman.Draw(spriteBatch, font);

        var deadBots = bots.Where(bot => bot.IsDead()).ToList();
        var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
        var deadPlayers = players.Where(player => player.IsDead()).ToList();
        var alivePlayers = players.Where(player => !player.IsDead()).ToList();

        foreach (var bot in deadBots)
        {
            bot.Draw(spriteBatch, font);
        }
        foreach (var player in deadPlayers)
        {
            player.Draw(spriteBatch, font);
        }
        foreach (var bot in aliveBots)
        {
            bot.Draw(spriteBatch, font);
        }
        alivePlayers = alivePlayers.OrderByDescending(player => player.spriteType).ToList();
        foreach (var player in alivePlayers)
        {
            player.Draw(spriteBatch, font);
        }
        debugManager.Draw(spriteBatch, font, players[0], players[0], bots);
        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
