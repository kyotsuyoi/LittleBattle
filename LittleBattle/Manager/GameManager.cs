using LittleBattle.Classes;
using LittleBattle.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
            new Sprite(new Vector2((Globals.Size.Width / 2) + 10, 504), Enums.SpriteType.Player1, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 3),
            //new Sprite(new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.SpriteType.Player2, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3),
        };

        bots = new List<Sprite>
        {
            new Sprite(new Vector2((Globals.Size.Width / 2) - 500, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 900, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3),
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
        InputManager.Update(players[0]);
        //InputManager.Update(players[1]);
        players[0].Update();
        players[0].UpdateSpriteFXDamage(bots);
        //player2.Update();
        foreach (var bot in bots)
        {
            botManager.Update();
            bot.Update();
            bot.UpdateSpriteFXDamage(players);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _canvas.Activate();

        spriteBatch.Begin();
        backgroundManager.Draw();
        foreach (var bot in bots)
        {
            bot.Draw(spriteBatch, font);
        }
        //player2.Draw();
        players[0].Draw(spriteBatch, font);
        debugManager.Draw(spriteBatch, font, players[0], players[0], bots);
        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
