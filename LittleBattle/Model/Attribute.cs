using LittleBattle.Classes;

namespace LittleBattle.Model
{
    public class Attribute
    {
        public int BaseHP { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public float CurrentSpeed { get; set; }
        public float Speed { get; set; }
        public float JumpPower { get; set; }
        public float BaseJumpPower { get; set; }
        public float AttackCooldown { get; set; }
        public float BaseAttackCooldown { get; set; }
        public float Knockback { get; set; }
        public Enums.Side KnockbackSide { get; set; }
        public int Range { get; set; }
        public float ComboTimeLimit { get; set; }
        public float BaseComboTimeLimit { get; set; }
        public float StuntTime { get; set; }


        public int BuffAttack { get; set; }
        public int BuffKnockback { get; set; }

        public Attribute(Enums.ClassType classType)
        {
            BaseHP = 100;
            HP = BaseHP;

            Attack = 2;
            Defense = 1;

            CurrentSpeed = 0;
            Speed = 2;
            JumpPower = 0;
            BaseJumpPower = 5;
            AttackCooldown = 0;
            BaseAttackCooldown = 0.5f;

            Knockback = 0;
            KnockbackSide = Enums.Side.None;

            Range = 5;

            ComboTimeLimit = 0;
            BaseComboTimeLimit = 0.5f;

            StuntTime = 0;

            if(classType == Enums.ClassType.Warrior)
            {
                BaseHP = 200;
                HP = BaseHP;

                Attack = 4;
                Defense = 2;
            }

            BuffAttack = 0;
            BuffKnockback = 0;
        }
    }
}
