using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Windows.UI.ViewManagement;

namespace LittleBattle.Classes
{
    public class Resolution
    {
        private Game game;
        private Canvas canvas;
        private GraphicsDeviceManager graphics;

        public Resolution(Game game, GraphicsDeviceManager graphics, Canvas canvas) {
            this.game = game;
            this.graphics = graphics;
            this.canvas = canvas;
        }

        public void SetResolution(Size size)
        {
            graphics.PreferredBackBufferWidth = size.Width;
            graphics.PreferredBackBufferHeight = size.Height;
            //graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            canvas.SetDestinationRectangle();
        }

        public void SetBorderlessScreen()
        {
            //game.Window.IsBorderless = !game.Window.IsBorderless;
            //graphics.IsFullScreen = false;
            //graphics.ApplyChanges();
            //canvas.SetDestinationRectangle();
        }

        public void SetFullScreen()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            canvas.SetDestinationRectangle();
        }

        private void WindowResize(int height, int width)
        {
            //var size = new Size(height, width);
            //ApplicationView.PreferredLaunchViewSize = size;
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //Window.Current.CoreWindow.SizeChanged -= (s, e) =>
            //{
            //    ApplicationView.GetForCurrentView().TryResizeView(size);
            //};
        }
    }
}
