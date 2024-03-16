using System.Collections.Generic;
using System.Linq;
using static LittleBattle.Classes.Enums;

namespace LittleBattle.Classes
{
    public class Bag
    {
        private List<Item> Items;
        //private List<Item> NeededItems;

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
                if (inner_item.Quantity - Quantity >= 0) {
                    inner_item.Quantity -= Quantity;
                    return true; 
                }
            }
            return false;
        }

        public bool UseItemsFor(SpriteType spriteType)
        {
            var NeededItems = Needs(spriteType);
            if (NeededItems.Count <= 0) return false;

            bool hasAll = true;
            foreach (var needdedItem in NeededItems)
            {
                var items = Items.Where(inner_item => inner_item.spriteType == needdedItem.spriteType).ToList();
                if (items.Count() <= 0) return false;
                if(items[0].Quantity - needdedItem.Quantity >= 0)
                {
                    hasAll = hasAll && true;
                }
                else
                {
                    return false;
                }
            }
            if (hasAll)
            {
                foreach (var needdedItem in NeededItems)
                {
                    UseItem(needdedItem.spriteType, needdedItem.Quantity);
                }
            }

            return hasAll;
        }

        public bool UseItemsFor(ClassType classType)
        {
            var NeededItems = Needs(classType);
            if (NeededItems.Count <= 0) return false;

            bool hasAll = true;
            foreach (var needdedItem in NeededItems)
            {
                var items = Items.Where(inner_item => inner_item.spriteType == needdedItem.spriteType).ToList();
                if (items.Count() <= 0) return false;
                if (items[0].Quantity - needdedItem.Quantity >= 0)
                {
                    hasAll = hasAll && true;
                }
                else
                {
                    return false;
                }
            }
            if (hasAll)
            {
                foreach (var needdedItem in NeededItems)
                {
                    UseItem(needdedItem.spriteType, needdedItem.Quantity);
                }
            }

            return hasAll;
        }

        private List<Item> Needs(SpriteType spriteType)
        {
            List<Item> NeededItems = null;
            switch (spriteType)
            {
                case SpriteType.Seed01:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Seed01, 1)
                    };
                    break;

                case SpriteType.Seed02:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Seed02, 1)
                    };
                    break;

                case SpriteType.ArcherTowerBuilding:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Wood, 12)
                    };
                    break;

                case SpriteType.Digging:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Wood, 8),
                        new Item(SpriteType.Vine, 10)
                    };
                    break;

                case SpriteType.WorkStationBuilding:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Wood, 30),
                        new Item(SpriteType.Vine, 26),
                        new Item(SpriteType.ToolBag, 1)
                    };
                    break;

                case SpriteType.ReferencePointBuilding:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Wood, 20),
                        new Item(SpriteType.Vine, 10)
                    };
                    break;
            }
            return NeededItems;
        }

        private List<Item> Needs(ClassType classType)
        {
            List<Item> NeededItems = null;
            switch (classType)
            {
                case ClassType.Newbie:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Fruit, 10)
                    };
                    break;

                case ClassType.Worker:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Iron, 2),
                        new Item(SpriteType.Vine, 6),
                        new Item(SpriteType.Wood, 10)
                    };
                    break;

                case ClassType.Warrior:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Iron, /*16*/1),
                        //new Item(SpriteType.Rope, 20),
                        new Item(SpriteType.Wood, 12)
                    };
                    break;

                case ClassType.Archer:
                    NeededItems = new List<Item>
                    {
                        new Item(SpriteType.Iron, 6),
                        new Item(SpriteType.Rope, 16),
                        new Item(SpriteType.Wood, 30)
                    };
                    break;
            }
            return NeededItems;
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