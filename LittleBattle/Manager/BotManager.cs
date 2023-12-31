using LittleBattle.Classes;
using System;
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
            Patrol();
            foreach (var bot in aliveBots)
            {
                var target = SetTarget(bot);
                if(target != null)
                {
                    TargetDistance(bot, target);
                    TargetAttack(bot, target);
                }
                else
                {
                    bot.SetMovement(false, Enums.Side.None);
                }
            }
        }

        private Sprite SetTarget(Sprite bot)
        {
            Sprite target = null;
            var alivePlayers = players.Where(player => !player.IsDead() && player.Team != bot.Team).ToList();
            var aliveBots_enemy = bots.Where(enemyBot => !enemyBot.IsDead() && enemyBot.Team != bot.Team).ToList();
            var allEnemies = aliveBots_enemy;

            foreach (var player in alivePlayers)
            {
                allEnemies.Add(player);
            }

            if (allEnemies.Count() <= 0) return target;
            target = allEnemies[0];
            float distance = 0;

            if (bot.RelativeX >= target.RelativeX)
            {
                distance = bot.RelativeX - target.RelativeX;
            }
            if (bot.RelativeX < target.RelativeX)
            {
                distance = target.RelativeX - bot.RelativeX;
            }

            foreach (var enemy in allEnemies)
            {
                float inner_distace = 0;
                if (bot.RelativeX >= enemy.RelativeX)
                {
                    inner_distace = bot.RelativeX - enemy.RelativeX;
                }
                if (bot.RelativeX < enemy.RelativeX)
                {
                    inner_distace = enemy.RelativeX - bot.RelativeX;
                }
                if (inner_distace < distance)
                {
                    distance = inner_distace;
                    target = enemy;
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
                    //bot.SetMovement(Enums.Direction.WalkLeft);
                    bot.SetMovement(true, Enums.Side.Left);
                }
                else
                {
                    //bot.SetMovement(Enums.Direction.StandLeft);
                    bot.SetMovement(false, Enums.Side.Left);
                }
            }

            if (bot.RelativeX < player.RelativeX)
            {
                if ((player.RelativeX + player.Size.X - bot.RelativeX < 200)
                    && ((bot.RelativeX + bot.Size.X) * 1 < player.RelativeX))
                {
                    //bot.SetMovement(Enums.Direction.WalkRight);
                    bot.SetMovement(true, Enums.Side.Right);
                }
                else
                {
                    //bot.SetMovement(Enums.Direction.StandRight);
                    bot.SetMovement(false, Enums.Side.Right);
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

        private void Patrol()
        {
            Random random = new Random();
            int randomVal = random.Next(400) * 1 - 200;
        }
    }
}
