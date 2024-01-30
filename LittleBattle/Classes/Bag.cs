using System.Collections.Generic;
using System.Linq;
using static LittleBattle.Classes.Enums;

namespace LittleBattle.Classes
{
    public class Bag
    {
        private List<Item> Items;

        public Bag() 
        { 
            Items = new List<Item>();
        }

        public void AddItem(SpriteType spriteType, int Quantity)
        {
            var inner_item = Items.FirstOrDefault(item => item.spriteType == spriteType);
            if (inner_item != null)
            {
                inner_item.Quantity += Quantity;
            }
            else
            {
                Items.Add(new Item(spriteType, Quantity));
            }
        }

        public int GetItemQuantity(SpriteType spriteType)
        {
            var inner_item = Items.FirstOrDefault(item => item.spriteType == spriteType);
            if (inner_item != null)
            {
                return inner_item.Quantity;
            }
            return 0;
        }

        public bool UseItem(SpriteType spriteType, int Quantity)
        {
            var inner_item = Items.FirstOrDefault(item => item.spriteType == spriteType);
            if (inner_item != null)
            {
                inner_item.Quantity -= Quantity;
                if (inner_item.Quantity < 0) {
                    inner_item.Quantity = 0;
                    return false; 
                }
                return true;
            }
            return false;
        }
    }

    public class Item
    {
        public SpriteType spriteType;
        public int Quantity;

        public Item(SpriteType spriteType, int Quantity)
        {
            this.spriteType = spriteType;
            this.Quantity = Quantity;
        }
    }
}