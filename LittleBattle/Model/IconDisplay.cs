using LittleBattle.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IconDisplay(SpriteType spriteType, int Quantity)
        {
            this.spriteType = spriteType;
            this.Quantity = Quantity;
            Time = 4f;
            this.PositionY = PositionY;
            Alpha = 1f;
        }

        public IconDisplay(Enums.Action action)
        {
            this.action = action;
            Time = 1f;
        }
    }
}
