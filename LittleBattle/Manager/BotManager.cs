using LittleBattle.Classes;
using System.Collections.Generic;
using System.Numerics;

namespace LittleBattle.Manager
{
    public class BotManager
    {
        private List<Sprite> bots;
        private Sprite player;

        public BotManager(List<Sprite> bots, Sprite player)
        {
            this.bots = bots;
            this.player = player;
        }

        public void Update()
        {
            foreach (var bot in bots)
            {
                TargetDistance(bot);
                TargetAttack(bot);
            }
        }

        private void TargetDistance(Sprite bot)
        {
            if (bot.RelativeX >= player.RelativeX)
            {
                if ((bot.RelativeX - (player.RelativeX + player.Size.X) < 200)
                    && (player.RelativeX + player.Size.X) * 1 < bot.RelativeX)
                {
                    bot.Direction = Enums.Direction.WalkLeft;
                    bot.Walk = true;
                }
                else
                {
                    bot.Direction = Enums.Direction.StandLeft;
                    bot.Walk = false;
                }
            }

            if (bot.RelativeX < player.RelativeX)
            {
                if ((player.RelativeX + player.Size.X - bot.RelativeX < 200)
                    && ((bot.RelativeX + bot.Size.X) * 1 < player.RelativeX))
                {
                    bot.Direction = Enums.Direction.WalkRight;
                    bot.Walk = true;
                }
                else
                {
                    bot.Direction = Enums.Direction.StandRight;
                    bot.Walk = false;
                }
            }
        }

        private void TargetAttack(Sprite bot)
        {

            Collision collision = new Collision();
            var collide = collision.SquareCollision(
                new Vector2((int)bot.RelativeX, 0),
                bot.Size * new Vector2(1.2f, 1.2f),
                new Vector2((int)player.RelativeX, 0),
                player.Size * new Vector2(1.2f, 1.2f)
            );

            if (collide)
            {
                bot.Attack = true;
            }
        }
    }
}
