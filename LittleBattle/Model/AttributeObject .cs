using LittleBattle.Classes;

namespace LittleBattle.Model
{
    public class AttributeObject
    {
        public int BaseHP { get; set; }
        public int HP { get; set; }

        public AttributeObject()
        {
            BaseHP = 15;
            HP = BaseHP;
        }
    }
}
