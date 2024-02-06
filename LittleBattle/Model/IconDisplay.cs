using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using static LittleBattle.Classes.Enums;

namespace LittleBattle.Model
{
    public class IconDisplay
    {
        public SpriteType spriteType { get; set; }
        public Enums.Action action { get; set; }
        public int Quantity { get; set; }
        public float Time { get; set; }
        public float PositionY { get; set; }
        public float Alpha { get; set; }

        public SpriteObject spriteObject { get; }

        public IconDisplay(SpriteType spriteType, int Quantity, SpriteObject spriteObject)
        {
            this.spriteType = spriteType;
            this.Quantity = Quantity;
            Time = 4f;
            //this.PositionY = PositionY;
            Alpha = 1f;
            this.spriteObject = spriteObject;
        }

        public IconDisplay(SpriteType spriteType, SpriteObject spriteObject)
        {
            this.action = action;
            Time = 1f;
            this.spriteObject = spriteObject;
        }

        public void Reset(Vector2 Position)
        {
            PositionY = 0;
            Time = 4f;
            Alpha = 1f;
            spriteObject.Position = Position;
        }
    }
}
