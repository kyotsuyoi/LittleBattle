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
            BaseHP = 10;
            HP = BaseHP;

            Attack = 1;
            Defense = 0;

            Speed = 1;
            Range = 5;

            BaseJumpPower = 5;

            if (classType == Enums.ClassType.Warrior)
            {
                BaseHP = 200;
                HP = BaseHP;

                Attack = 4;
                Defense = 2;
            }

            if (classType == Enums.ClassType.Archer)
            {
                BaseHP = 100;
                HP = BaseHP;

                Attack = 2;
                Defense = 1;
            }

            if (classType == Enums.ClassType.Worker)
            {
                BaseHP = 100;
                HP = BaseHP;

                Attack = 1;
                Defense = 1;
            }

            if (classType == Enums.ClassType.Newbie)
            {
                Speed *= 1.15f;
                BaseJumpPower = 3;
            }

            ComboTimeLimit = 0;
            BaseComboTimeLimit = 0.5f;

            CurrentSpeed = 0;
            JumpPower = 0;
            AttackCooldown = 0;
            BaseAttackCooldown = 0.5f;

            Knockback = 0;
            KnockbackSide = Enums.Side.None;

            StuntTime = 0;

            BuffAttack = 0;
            BuffKnockback = 0;
        }
    }
}
