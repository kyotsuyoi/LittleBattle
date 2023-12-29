using LittleBattle.Classes;
using LittleBattle.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class GameManager
{
    private readonly Sprite player1;
    private readonly Sprite player2;
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
        player1 = new Sprite(new Vector2((Globals.Size.Width / 2) + 10, 504), Enums.SpriteType.Player1, Globals.Content.Load<Texture2D>("Sprites/Sprite01_x3"), 4, 3);
        player2 = new Sprite(new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.SpriteType.Player2, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3);
        bots = new List<Sprite>
        {
            new Sprite(new Vector2((Globals.Size.Width / 2) - 500, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3),
            new Sprite(new Vector2((Globals.Size.Width / 2) - 900, 504), Enums.SpriteType.Bot, Globals.Content.Load<Texture2D>("Sprites/Sprite02_x3"), 4, 3)
        };
    
        Globals.Gravity = 10;

        font = Globals.Content.Load<SpriteFont>("Font/fontMedium");
        debugManager = new DebugManager();
        botManager = new BotManager(bots, player1);
;    }    

    public void Update()
    {
        InputManager.UpdateResolution(resolution);
        Globals.CameraMovement = player1.DirectionSpeed();
        backgroundManager.Update();
        InputManager.Update(player1);
        InputManager.Update(player2);
        player1.Update();
        //player2.Update();
        foreach (var bot in bots)
        {
            botManager.Update();
            bot.Update();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _canvas.Activate();

        spriteBatch.Begin();
        backgroundManager.Draw();
        foreach (var bot in bots)
        {
            bot.Draw();
        }
        //player2.Draw();
        player1.Draw();
        debugManager.Draw(spriteBatch, font, player1, player2, bots);
        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
