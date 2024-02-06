using Microsoft.Xna.Framework;
using System;
using static LittleBattle.Classes.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LittleBattle.Classes
{
    public class Common
    {
        public int RandomQuantity(int max, int min = 0)
        {
            int randomVal;
            Random random = new Random();
            randomVal = random.Next(max + 1) * 1 + min;
            return randomVal;
        }

        public Vector2 RandomPosition()
        {
            int randomValX, randomValY;
            Random random = new Random();
            randomValX = random.Next(120) * 1 - 60;
            randomValY = random.Next(60) * 1 - 30;
            return new Vector2(randomValX, randomValY);
        }

        public List<SpriteObjectItem> RandomObjects(List<SpriteObjectItem> new_objects, SpriteType spriteType, Vector2 position)
        {
            if (spriteType == SpriteType.Tree01)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(8, 1)));
                //new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Seed01, position + RandomPosition(), RandomQuantity(2,1)));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Vine, position + RandomPosition(), RandomQuantity(10)));
            }
            if (spriteType == SpriteType.Tree02)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Wood, position + RandomPosition(), RandomQuantity(2, 1)));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Seed02, position + RandomPosition(), RandomQuantity(1, 1)));
            }
            if (spriteType == SpriteType.ResourceStone)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Stone, position + RandomPosition(), RandomQuantity(8, 1)));
            }
            if (spriteType == SpriteType.ResourceIron)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Stone, position + RandomPosition(), RandomQuantity(2, 1)));
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Iron, position + RandomPosition(), RandomQuantity(3)));
            }
            if (spriteType == SpriteType.Fruit)
            {
                new_objects.Add(new SpriteObjectItem(null, Side.None, SpriteType.Fruit, position + RandomPosition(), RandomQuantity(1, 0)));
            }
            new_objects = new_objects.Where(item => item.Quantity > 0).ToList();
            return new_objects;
        }

        public bool PercentualCalc(int limit)
        {
            Random random = new Random();
            int randomVal = random.Next(100 + 1) * 1 + 0;
            if (randomVal <= limit) return true;
            return false;
        }
    }
}
