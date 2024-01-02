namespace LittleBattle.Model
{
    public class AttributeFX
    {
        public int Damage { get; set; }
        public int Knockback { get; set; }
        public int Range { get; set; }
        public float StuntTime { get; set; }

        public AttributeFX()
        {
            Damage = 2; 
            Knockback = 5;
            Range = 5;
            StuntTime = 0;
        }
    }
}
