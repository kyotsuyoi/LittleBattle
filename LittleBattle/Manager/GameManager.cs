using LittleBattle.Classes;
using LittleBattle.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        Globals.Size = new System.Drawing.Size(1920, 1080);
        _canvas = new Canvas(graphics.GraphicsDevice, Globals.Size.Width, Globals.Size.Height);
        backgroundManager = new BackgroundManager();
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Trees"), 0.0f, 0.8f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Background/Ground"), 1f, 1f, false));

        resolution = new Resolution(game, graphics, _canvas);
        resolution.SetResolution(Globals.Size);
        resolution.SetFullScreen();
        players = new List<Sprite>
        {
            new Sprite(new Vector2((Globals.Size.Width / 2) + 10, 504), Enums.SpriteType.Player1, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            //new Sprite(new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.SpriteType.Player2, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
        };

        bots = new List<Sprite>
        {
            new Sprite(new Vector2((Globals.Size.Width / 2) - 500, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 600, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 700, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 800, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 4, Enums.Team.Team2),

            new Sprite(new Vector2((Globals.Size.Width / 2) - 400, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 450, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 455, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 460, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 4, Enums.Team.Team1),
        };
    
        Globals.Gravity = 10;

        font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
        debugManager = new DebugManager();
        botManager = new BotManager(bots, players);
;    }    

    public void Update()
    {
        InputManager.UpdateResolution(resolution);
        Globals.CameraMovement = players[0].DirectionSpeed();
        backgroundManager.Update();

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
