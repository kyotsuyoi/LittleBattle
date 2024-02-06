using LittleBattle.Classes;
using LittleBattle.Manager;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml.Controls;
using static LittleBattle.Classes.Enums;

public class SpriteObjectItem : SpriteObject
{
    public int Quantity { get; set; }

    public SpriteObjectItem(Sprite Owner, Side side, SpriteType spriteType, Vector2 initialPosition, int Quantity) : base(Owner, side, spriteType, initialPosition)
    {
        this.Quantity = Quantity;
    }

}
