using static LittleBattle.Classes.Enums;

namespace LittleBattle.Model
{
    public class AttributeObject
    {
        public int BaseHP { get; set; }
        public float HP { get; set; }

        public int MaxBuild { get; set; }
        public float Build { get; set; }

        private SpriteType spriteType;

        public AttributeObject(SpriteType spriteType)
        {
            this.spriteType = spriteType;
            SetBaseHP();
            SetMaxBuild();

            //BaseHP = 10;
            HP = BaseHP;

            //MaxBuild = 240;
            Build = 0;
        }

        private void SetMaxBuild()
        {
            switch (spriteType)
            {
                case SpriteType.GrowingTree:
                    MaxBuild = 240;
                    break;

                case SpriteType.ArcherTowerBuild:
                    MaxBuild = 30;
                    break;

                default:
                    MaxBuild = 1;
                    break;
            }
        }

        public void SetBaseHP()
        {
            switch (spriteType)
            {
                case SpriteType.ArcherTower:
                    BaseHP = 100;
                    break;

                case SpriteType.Tree01:
                    BaseHP = 10;
                    break;

                case SpriteType.Resource:
                    BaseHP = 5;
                    break;

                default:
                    BaseHP = 1;
                    break;
            }
        }
    }
}
