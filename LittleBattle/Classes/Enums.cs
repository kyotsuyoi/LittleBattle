using Microsoft.Xna.Framework;

namespace LittleBattle.Classes
{
    public static class Enums
    {
        public static class Direction
        {
            //Up,    
            //public static Vector2 Down = new Vector2(0, 0); 
            public static Vector2 None = new Vector2(0, 0); 
            public static Vector2 WalkRight = new Vector2(1, 0);   
            public static Vector2 WalkLeft = new Vector2(-1, 0);

            public static Vector2 StandRight = new Vector2(0.1f, 0);
            public static Vector2 StandLeft = new Vector2(-0.1f, 0);
            public static Vector2 AttackRight = new Vector2(0.2f, 0);
            public static Vector2 AttackLeft = new Vector2(-0.2f, 0);
            public static Vector2 DeadRight = new Vector2(0.3f, 0);
            public static Vector2 DeadLeft = new Vector2(-0.3f, 0);
            public static Vector2 StuntRight = new Vector2(0.4f, 0);
            public static Vector2 StuntLeft = new Vector2(-0.4f, 0);
        }

        public enum Side
        {
            None = 0,
            Right = 1,
            Left = -1,
        }

        public enum ControlType
        {
            KeyBoard = 0,
            GamePad = 1
        }

        //public static class ControlKeys
        //{
        //    public const Keys Left = Keys.Left;
        //    public const Keys Right = Keys.Right;
        //    public const Keys Jump = Keys.Up;
        //    public const Keys Action = Keys.Down;
        //    public const Keys Shoot = Keys.Space;
        //}

        public enum SpriteType
        {
            None = 0,
            Player1 = 1,
            Player2 = 2,
            Bot = 5,
            Cameraman = 6,
            SwordEffect = 50,
            ArrowEffect = 51,
        }

        public enum Team
        {
            None = 0,
            Team1 = 1,
            Team2 = 2,
            Team3 = 3,
            Team4 = 4,
        }

        public enum ClassType
        {
            None = 0,
            Warrior = 1,
            Archer = 2,
        }
    }
}
