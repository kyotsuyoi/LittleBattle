using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

public class GameManager
{
    private readonly Sprite player1;
    private readonly Sprite player2;
    private readonly Canvas _canvas;
    private readonly Resolution resolution;
    private readonly BackgroundManager backgroundManager = new BackgroundManager();

    public GameManager(Game game, GraphicsDeviceManager graphics)
    {
        Globals.Size = new System.Drawing.Size(1920, 1080);
        _canvas = new Canvas(graphics.GraphicsDevice, Globals.Size.Width, Globals.Size.Height);
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Trees"), 0.0f, 0.8f, false));
        backgroundManager.AddLayer(new Layer(Globals.Content.Load<Texture2D>("Ground"), 1f, 1f, false));

        resolution = new Resolution(game, graphics, _canvas);
        resolution.SetResolution(Globals.Size);
        resolution.SetFullScreen();
        player1 = new Sprite(new Vector2((Globals.Size.Width / 2) + 10, 504), Enums.Player.Player1);
        player2 = new Sprite(new Vector2((Globals.Size.Width / 2) - 10, 504), Enums.Player.Player2);
        Globals.Gravity = 10;
    }    

    public void Update()
    {
        InputManager.UpdateResolution(resolution);
        backgroundManager.Update(player1.DirectionSpeed());
        InputManager.Update(player1);
        InputManager.Update(player2);
        player1.Update();
        player2.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _canvas.Activate();

        spriteBatch.Begin();
        backgroundManager.Draw();
        player1.Draw();
        player2.Draw();
        spriteBatch.End();

        _canvas.Draw(spriteBatch);
    }
}
