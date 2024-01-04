using Microsoft.Xna.Framework;

namespace LittleBattle.Manager
{
    public class Collision
    {
        public float contactLeft = 0;
        public float contactRight = 0;
        public float contactTop = 0;
        public float contactBottom = 0;

        public bool IsCollide(Rectangle A, Rectangle B)
        {
            if (A.Left - 1 < B.Right && A.Right + 1 > B.Left &&
                A.Top - 1 < B.Bottom && A.Bottom + 1 > B.Top) return true;
            return false;
        }

        private float contactHeight(Rectangle A, Rectangle B)
        {
            float p1, p2;

            p1 = A.Top;
            if (B.Top > A.Top)
            {
                p1 = B.Top;
            }

            p2 = A.Bottom;
            if (B.Bottom < A.Bottom)
            {
                p2 = B.Bottom;
            }
            return p2 - p1;
        }

        private float contactWidth(Rectangle A, Rectangle B)
        {
            float p1, p2;

            p1 = A.Left;
            if (B.Left > A.Left)
            {
                p1 = B.Left;
            }

            p2 = A.Right;
            if (B.Right < A.Right)
            {
                p2 = B.Right;
            }
            return p2 - p1;
        }

        public void CheckCollisionSide(Rectangle A, Rectangle B)
        {
            if (IsCollide(A, B))
            {
                if (A.Right >= B.Left && A.Right <= B.Right)
                {
                    contactLeft = contactHeight(A, B);
                }

                if (A.Left <= B.Right && A.Left >= B.Left)
                {
                    contactRight = contactHeight(A, B);
                }

                if (A.Bottom >= B.Top && A.Bottom <= B.Bottom)
                {
                    contactBottom = contactWidth(A, B);
                }

                if (A.Top <= B.Bottom && A.Top >= B.Top)
                {
                    contactTop = contactWidth(A, B);
                }
            }
        }

        public bool SquareCollision(Vector2 PositionA, Vector2 SizeA, Vector2 PositionB, Vector2 SizeB)
        {
            if (
                PositionA.X < PositionB.X + SizeB.X &&
                PositionA.X + SizeA.X > PositionB.X &&
                PositionA.Y < PositionB.Y + SizeB.Y &&
                PositionA.Y + SizeA.Y > PositionB.Y
            )
            {
                return true;
            }
            return false;
        }
    }
}
