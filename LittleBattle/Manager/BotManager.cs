﻿using LittleBattle.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Windows.UI.Xaml.Controls;

namespace LittleBattle.Manager
{
    public class BotManager
    {
        private Sprite Cameraman;
        private List<Sprite> bots;
        private List<Sprite> players;

        private float lastSecond = 0;

        public BotManager(Sprite Cameraman, List<Sprite> bots, List<Sprite> players)
        {
            this.Cameraman = Cameraman;
            this.bots = bots;
            this.players = players;
        }

        public void Update()
        {
            var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
            foreach (var bot in aliveBots)
            {
                var target = SetTarget(bot);
                if(target != null){
                    TargetAttack(bot, target);
                }
                if (target != null && TargetDistance(bot, target))
                {
                    //TargetDistance(bot, target);
                    //TargetAttack(bot, target);
                }
                else
                {
                    Patrol(bot);
                }
            }
        }

        public void UpdateCamerman()
        {
            var player_position_side = players[0].Position;
            if (Cameraman.GetSide() != players[0].GetSide())
            {
                Cameraman.Attribute.Speed = 4;
                lastSecond = Globals.TotalSeconds - 1f;
            }
            if (players[0].GetSide() == Enums.Side.Right)
            {
                player_position_side *= new Vector2(1.25f, 1);
            }
            if (players[0].GetSide() == Enums.Side.Left)
            {
                player_position_side /= new Vector2(1.25f, 1);
            }

            var stop = false;
            if (players[0].GetSide() == Enums.Side.Right)
            {
                if (Cameraman.Position.X > player_position_side.X)
                {
                    stop = true;
                }
            }
            if (players[0].GetSide() == Enums.Side.Left)
            {
                if (Cameraman.Position.X < player_position_side.X)
                {
                    stop = true;
                }
            }

            //Collision collision = new Collision();
            //var collide = collision.SquareCollision(
            //    Cameraman.Position,
            //    Cameraman.Size,
            //    player_position_side,
            //    players[0].Size
            //);

            if (stop)
            {
                Cameraman.Attribute.Speed = 1;
                Cameraman.SetMovement(false, Enums.Side.None);
                return;
            }

            if (Globals.TotalSeconds - lastSecond < 1f)
            {
                return;
            }

            lastSecond = Globals.TotalSeconds;

            if (players[0].GetSide() == Enums.Side.Left)
            {
                Cameraman.Attribute.Speed += 0.5f;
                Cameraman.SetMovement(true, Enums.Side.Left);
            }

            if (players[0].GetSide() == Enums.Side.Right)
            {
                Cameraman.Attribute.Speed += 0.5f;
                Cameraman.SetMovement(true, Enums.Side.Right);
            }

            //if ((Cameraman.RelativeX + Cameraman.Size.X) /2  > (player_position_side.X + players[0].Size.X) /2)
            //{
            //    Cameraman.Attribute.Speed += 0.1f;
            //    Cameraman.SetMovement(true, Enums.Side.Left);                
            //}

            //if ((Cameraman.RelativeX + Cameraman.Size.X) /2 < (player_position_side.X + players[0].Size.X) /2)
            //{
            //    Cameraman.Attribute.Speed += 0.1f;
            //    Cameraman.SetMovement(true, Enums.Side.Right);                
            //}
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

        private bool TargetDistance(Sprite bot, Sprite target)
        {
            bool range = false;
            if (target == null) return false;
            if (bot.RelativeX + bot.Size.X >= target.RelativeX)
            {
                if ((bot.RelativeX - (target.RelativeX + target.Size.X) < 400)
                    && (target.RelativeX + target.Size.X) * 1 < bot.RelativeX)
                {
                    bot.SetMovement(true, Enums.Side.Left);
                    range = true;
                }
            }

            if (bot.RelativeX < target.RelativeX + target.Size.X)
            {
                if ((target.RelativeX + target.Size.X - bot.RelativeX < 400)
                    && ((bot.RelativeX + bot.Size.X) * 1 < target.RelativeX))
                {
                    bot.SetMovement(true, Enums.Side.Right);
                    range = true;
                }
            }
            return range;
        }

        private void TargetAttack(Sprite bot, Sprite player)
        {
            Collision collision = new Collision();
            var collide = collision.SquareCollision(
                new Vector2((int)bot.RelativeX*0.8f, 0),
                bot.Size * new Vector2(1.2f, 1),
                new Vector2((int)player.RelativeX*0.8f, 0),
                player.Size * new Vector2(1.2f, 1)
            );

            if (collide)
            {
                bot.SetAttack();
            }
        }

        private void Patrol(Sprite bot)
        {
            bot.BotPatrolWait -= 0.1f * Globals.ElapsedSeconds;
            if (bot.BotPatrolWait > 0) return;
            if (bot.BotPatrol == 0)
            {
                Random random = new Random();
                int randomVal = random.Next(400) * 1 - 200;
                bot.BotPatrol = randomVal;
                bot.SetMovement(false, Enums.Side.None);
            }
            else
            {
                if (bot.RelativeX > bot.BotPatrol)
                {
                    bot.SetMovement(true, Enums.Side.Left);
                }

                if (bot.RelativeX < bot.BotPatrol)
                {
                    bot.SetMovement(true, Enums.Side.Right);
                }

                if ((int)bot.RelativeX == (int)bot.BotPatrol)
                {
                    bot.SetMovement(false, Enums.Side.None);
                    bot.BotPatrol = 0;
                    bot.BotPatrolWait = 5;
                }
            }
        }
    }
}
