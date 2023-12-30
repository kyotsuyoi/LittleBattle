using LittleBattle.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace LittleBattle.Manager
{
    public class BotManager
    {
        private List<Sprite> bots;
        private List<Sprite> players;

        public BotManager(List<Sprite> bots, List<Sprite> players)
        {
            this.bots = bots;
            this.players = players;
        }

        public void Update()
        {
            var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
            foreach (var bot in aliveBots)
            {
                var target = SetTarget(bot);
                if(target != null)
                {
                    TargetDistance(bot, target);
                    TargetAttack(bot, target);
                }
                //if (player.Attribute.HP > 0)
                //{
                //    TargetDistance(bot, player);
                //    TargetAttack(bot, player);
                //}
            }
        }

        private Sprite SetTarget(Sprite bot)
        {
            Sprite target = null;
            var alivePlayers = players.Where(player => !player.IsDead()).ToList();
            if (alivePlayers.Count() <= 0) return target;
            target = alivePlayers[0];
            float distance = 0;

            if (bot.RelativeX >= target.RelativeX)
            {
                distance = bot.RelativeX - target.RelativeX;
            }
            if (bot.RelativeX < target.RelativeX)
            {
                distance = target.RelativeX - bot.RelativeX;
            }

            foreach (var player in alivePlayers)
            {
                float inner_distace = 0;
                if (bot.RelativeX >= player.RelativeX)
                {
                    inner_distace = bot.RelativeX - player.RelativeX;
                }
                if (bot.RelativeX < player.RelativeX)
                {
                    inner_distace = player.RelativeX - bot.RelativeX;
                }
                if (inner_distace < distance)
                {
                    distance = inner_distace;
                    target = player;
                }
            }
            return target;
        }

        private void TargetDistance(Sprite bot, Sprite player)
        {
            if (bot.RelativeX >= player.RelativeX)
            {
                if ((bot.RelativeX - (player.RelativeX + player.Size.X) < 200)
                    && (player.RelativeX + player.Size.X) * 1 < bot.RelativeX)
                {
                    bot.SetMovement(Enums.Direction.WalkLeft);
                }
                else
                {
                    bot.SetMovement(Enums.Direction.StandLeft);
                }
            }

            if (bot.RelativeX < player.RelativeX)
            {
                if ((player.RelativeX + player.Size.X - bot.RelativeX < 200)
                    && ((bot.RelativeX + bot.Size.X) * 1 < player.RelativeX))
                {
                    bot.SetMovement(Enums.Direction.WalkRight);
                }
                else
                {
                    bot.SetMovement(Enums.Direction.StandRight);
                }
            }
        }

        private void TargetAttack(Sprite bot, Sprite player)
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
                bot.SetAttack();
            }
        }
    }
}
