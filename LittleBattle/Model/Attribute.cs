using LittleBattle.Classes;

namespace LittleBattle.Model
{
    public class Attribute
    {
        public int BaseHP { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public float Speed { get; set; }
        public float JumpPower { get; set; }
        public float BaseJumpPower { get; set; }
        public float AttackCooldown { get; set; }
        public float BaseAttackCooldown { get; set; }
        public float Knockback { get; set; }
        public Enums.Side KnockbackSide { get; set; }
        public int Range { get; set; }

        public Attribute()
        {
            BaseHP = 1000;
            HP = BaseHP;

            Attack = 2;
            Defense = 100;

            Speed = 2;
            JumpPower = 5;
            BaseJumpPower = 5;
            AttackCooldown = 0;
            BaseAttackCooldown = 0.5f;

            Knockback = 0;
            KnockbackSide = Enums.Side.None;

            Range = 5;
        }
    }
}
