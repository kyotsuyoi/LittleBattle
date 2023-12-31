using LittleBattle.Classes;
using LittleBattle.Manager;
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

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        Globals.Size = new System.Drawing.Size(1920, 1080);
        _canvas = new Canvas(graphics.GraphicsDevice, Globals.Size.Width, Globals.Size.Height);
        backgroundManager = new BackgroundManager();
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Mountain"), 0.6f, 0.6f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Trees"), 0.8f, 0.8f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Ground"), 1f, 1f, false));

        resolution = new Resolution(game, graphics, _canvas);
        resolution.SetResolution(Globals.Size);
        resolution.SetFullScreen();

        Cameraman = new Sprite(00, new Vector2((Globals.Size.Width / 2), 504), Enums.SpriteType.None, Globals.Content.Load<Texture2D>("Sprites/SpriteCameraman_x3"), 4, 4, Enums.Team.None);
        Cameraman.CenterX_Adjust();
        players = new List<Sprite>
        {
            new Sprite(01, new Vector2((Globals.Size.Width / 2), 504), Enums.SpriteType.Player1, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            //new Sprite(02, new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.SpriteType.Player2, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
        };
        players[0].CenterX_Adjust();

        bots = new List<Sprite>
        {
            new Sprite(10, new Vector2((Globals.Size.Width / 2) - 500, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(11, new Vector2((Globals.Size.Width / 2) - 600, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(12, new Vector2((Globals.Size.Width / 2) - 700, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(13, new Vector2((Globals.Size.Width / 2) - 800, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),

            new Sprite(14, new Vector2((Globals.Size.Width / 2) - 500, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(15, new Vector2((Globals.Size.Width / 2) - 600, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(16, new Vector2((Globals.Size.Width / 2) - 700, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(17, new Vector2((Globals.Size.Width / 2) - 800, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),

            new Sprite(18, new Vector2((Globals.Size.Width / 2) - 400, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(19, new Vector2((Globals.Size.Width / 2) - 300, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(20, new Vector2((Globals.Size.Width / 2) - 200, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(21, new Vector2((Globals.Size.Width / 2) - 100, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),

            new Sprite(22, new Vector2((Globals.Size.Width / 2) - 400, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(23, new Vector2((Globals.Size.Width / 2) - 300, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(24, new Vector2((Globals.Size.Width / 2) - 200, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(25, new Vector2((Globals.Size.Width / 2) - 100, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
        };
    
        Globals.Gravity = 10;
        Globals.PositiveLimit = new Size(2000, 0);
        Globals.NegativeLimit = new Size(-2000, 0);

        font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
        debugManager = new DebugManager();
        botManager = new BotManager(Cameraman, bots, players);
;    }    

    public void Update()
    {
        InputManager.UpdateResolution(resolution);
        //Globals.CameraMovement = players[0].DirectionSpeed();
        Globals.CameraMovement = Cameraman.DirectionSpeed();
        backgroundManager.Update();

        Cameraman.Update();
        botManager.UpdateCamerman();
        foreach(var player in players)
        {
            InputManager.Update(player, bots);
            player.Update();
            player.UpdateSpriteFXDamage(players);
            player.UpdateSpriteFXDamage(bots);
        }
        foreach (var bot in bots)
        {
            botManager.Update();
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
