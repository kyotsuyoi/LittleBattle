using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LittleBattle.Classes
{
    public static class Enums
    {
        public static class Direction
        {
            //Up,    
            //public static Vector2 Down = new Vector2(0, 0); 
            public static Vector2 WalkRight = new Vector2(1, 0);   
            public static Vector2 WalkLeft = new Vector2(-1, 0);
            public static Vector2 StandRight = new Vector2(0.1f, 0);
            public static Vector2 StandLeft = new Vector2(-0.1f, 0);
        }

        public static class ControlKeys
        {
            public const Keys Left = Keys.Left;
            public const Keys Right = Keys.Right;
            public const Keys Jump = Keys.Up;
            public const Keys Action = Keys.Down;
            public const Keys Shoot = Keys.Space;
        }

        public enum Player
        {
            None = 0,
            Player1 = 1,
            Player2 = 2,
        }
    }
}
