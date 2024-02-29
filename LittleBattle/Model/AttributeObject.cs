using static LittleBattle.Classes.Enums;

namespace LittleBattle.Model
{
    public class AttributeObject
    {
        public int BaseHP { get; set; }
        public float HP { get; set; }

        public int MaxBuild { get; set; }
        public float Build { get; set; }
        public float BuildNew { get; set; }

        private SpriteType spriteType;

        public int BuildNewCounter { get; set; }

        public AttributeObject(SpriteType spriteType)
        {
            this.spriteType = spriteType;
            SetBaseHP();
            SetMaxBuild();

            //BaseHP = 10;
            HP = BaseHP;

            //MaxBuild = 240;
            Build = 0;
            BuildNew = 0;
        }

        private void SetMaxBuild()
        {
            switch (spriteType)
            {
                case SpriteType.Tree01Growing:
                    MaxBuild = 240;
                    break;

                case SpriteType.Tree02Growing:
                    MaxBuild = 300;
                    break;

                case SpriteType.ArcherTowerBuilding:
                    MaxBuild = 30;
                    break;

                case SpriteType.Digging:
                    MaxBuild = 4;
                    break;

                case SpriteType.WorkStationBuilding:
                    MaxBuild = 50;
                    break;

                case SpriteType.ReferencePoint:
                    MaxBuild = 50;
                    break;

                case SpriteType.Tree02:
                    MaxBuild = 5;
                    break;

                case SpriteType.ReferencePointBuilding:
                    MaxBuild = 20;
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
                case SpriteType.Tree02:
                case SpriteType.Tree01MidLife:
                case SpriteType.Tree01EndLife:
                case SpriteType.Tree02MidLife:
                case SpriteType.Tree02EndLife:
                case SpriteType.TreeDried:
                    BaseHP = 10;
                    break;

                case SpriteType.ResourceStone:
                    BaseHP = 5;
                    break;

                case SpriteType.ResourceIron:
                    BaseHP = 5;
                    break;

                default:
                    BaseHP = 1;
                    break;
            }
        }
    }
}
