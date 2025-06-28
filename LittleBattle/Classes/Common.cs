using Microsoft.Xna.Framework;
using System;
using static LittleBattle.Classes.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LittleBattle.Classes
{
    public class Common
    {

        Random random = new Random();

        public int RandomQuantity(int max, int min = 0)
        {
            int randomVal;
            randomVal = random.Next(max + 1) * 1 + min;
            return randomVal;
        }

        public Vector2 RandomPosition()
        {
            int randomValX, randomValY;
            randomValX = random.Next(120) * 1 - 60;
            randomValY = random.Next(60) * 1 - 30;
            return new Vector2(randomValX, randomValY);
        }

        public List<SpriteObjectItem> RandomObjects(List<SpriteObjectItem> new_objects, SpriteType spriteType, Vector2 position, GraphicsDeviceManager graphics)
        {
            if (spriteType == SpriteType.Tree01 || spriteType == SpriteType.Tree01MidLife)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(8, 1), graphics));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Vine, position + RandomPosition(), RandomQuantity(10), graphics));
            }
            if (spriteType == SpriteType.Tree02 || spriteType == SpriteType.Tree02MidLife)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(2, 1), graphics));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Seed02, position + RandomPosition(), RandomQuantity(1), graphics));
            }
            if (spriteType == SpriteType.ResourceStone)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Stone, position + RandomPosition(), RandomQuantity(8, 1), graphics));
            }
            if (spriteType == SpriteType.ResourceIron)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Stone, position + RandomPosition(), RandomQuantity(2, 1), graphics));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Iron, position + RandomPosition(), RandomQuantity(3), graphics));
            }
            if (spriteType == SpriteType.Fruit)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Fruit, position + RandomPosition(), RandomQuantity(1), graphics));
            }
            if (spriteType == SpriteType.Tree01EndLife)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(2), graphics));
            }
            if (spriteType == SpriteType.Tree02EndLife)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(1), graphics));
            }
            //if (spriteType == SpriteType.TreeDried)
            //{
            //    new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(1)));
            //}
            new_objects = new_objects.Where(item => item.Quantity > 0).ToList();
            return new_objects;
        }

        public bool PercentualCalc(int limit)
        {
            int randomVal = random.Next(100 + 1) * 1 + 0;
            if (randomVal <= limit) return true;
            return false;
        }
    }
}
