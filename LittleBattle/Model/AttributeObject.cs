using LittleBattle.Classes;

namespace LittleBattle.Model
{
    public class AttributeObject
    {
        public int BaseHP { get; set; }
        public int HP { get; set; }

        public int MaxBuild { get; set; }
        public int Build { get; set; }

        public AttributeObject()
        {
            BaseHP = 150;
            HP = BaseHP;

            MaxBuild = 1000;
            Build = 0;
        }
    }
}
