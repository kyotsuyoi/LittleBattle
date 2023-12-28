using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteBase
{
    protected Texture2D texture;
    public Vector2 Position { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public SpriteBase(Texture2D texture, Vector2 position)
    {
        this.texture = texture;
        Position = position;
        Width = texture.Width;
        Height = texture.Height;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Position, Color.White);
    }
}
