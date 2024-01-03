using LittleBattle.Classes;
using SharpDX.Direct2D1;
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
        //private Sprite Cameraman;
        //private List<Sprite> bots;
        //private List<Sprite> players;

        private float lastSecond = 0;

        public BotManager()
        {
            //this.Cameraman = Cameraman;
            //this.bots = bots;
            //this.players = players;
        }

        public void Update(List<Sprite> bots, List<Sprite> players, bool goToCommand = false)
        {
            var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
            var test = bots.Where(bot => bot.Team != Enums.Team.Team1).ToList();

            foreach (var bot in aliveBots)
            {
                var target = SetTarget(bot, players, bots);
                if(target != null){
                    if (TargetAttack(bot, target)) goto _continue;
                }

                if (!TargetDistance(bot, target) || target == null)
                {
                    if (goToCommand && bot.Team == players[0].Team)
                    {
                        bot.BotPatrolX_Area = players[0].RelativeX;
                        bot.BotGoTo = true;
                    }
                    else if (bot.Team != players[0].Team)
                    {
                        bot.BotPatrol = true;
                    }

                    if (bot.BotGoTo)
                    {
                        GoTo(bot);
                    }
                    else
                    {
                        Patrol(bot);
                    }
                }
                _continue:;
            }
        }

        public void UpdateCamerman(Sprite Cameraman, List<Sprite> players)
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

        private Sprite SetTarget(Sprite bot, List<Sprite> players, List<Sprite> bots)
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

            if (bot.RelativeX < target.RelativeX)
            {
                if ((target.RelativeX - bot.RelativeX < 400)
                    && (bot.RelativeX < target.RelativeX))
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if(bot.RelativeX + bot.Size.X + bot.Attribute.Range > target.RelativeX)
                    {
                        bot.SetMovement(false, Enums.Side.Right);
                    }
                    else
                    {
                        bot.SetMovement(true, Enums.Side.Right);
                    }
                    range = true;
                }
            }

            if (bot.RelativeX + bot.Size.X >= target.RelativeX)
            {
                if ((bot.RelativeX - (target.RelativeX) < 400)
                    && (target.RelativeX) * 1 < bot.RelativeX)
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if (bot.RelativeX - bot.Size.X - bot.Attribute.Range < target.RelativeX)
                    {
                        bot.SetMovement(false, Enums.Side.Left);
                    }
                    else
                    {
                        bot.SetMovement(true, Enums.Side.Left);
                    }
                    range = true;
                }
            }

            return range;
        }

        private bool TargetAttack(Sprite bot, Sprite player)
        {
            Collision collision = new Collision();

            var m = 1.2f;
            var d = 0.8f;
            if (bot.classType == Enums.ClassType.Archer)
            {
                 m = 1.85f;
                 d = 0.15f;
            }

            var collide = collision.SquareCollision(
                new Vector2((int)bot.RelativeX * d, 0),
                bot.Size * new Vector2(m, 1),
                new Vector2((int)player.RelativeX * d, 0),
                player.Size * new Vector2(m, 1)
            );

            if (collide)
            {
                if (bot.RelativeX > player.RelativeX) bot.SetMovement(false, Enums.Side.Left);
                if (bot.RelativeX < player.RelativeX) bot.SetMovement(false, Enums.Side.Right);
                bot.SetAttack();
            }
            return collide;
        }

        private void Patrol(Sprite bot)
        {
            if (!bot.BotPatrol) return;
            if (bot.BotPatrolWait > 0)
            {
                bot.BotPatrolWait -= Globals.ElapsedSeconds;
                return;
            }

            if (bot.BotPatrolX == 0)
            {
                int randomVal;
                Random random = new Random();
                randomVal = random.Next(400) * 1 - 200;
                bot.BotPatrolX = randomVal;
                bot.SetMovement(false, Enums.Side.None);
                
                bot.BotPatrolX = randomVal;
                bot.SetMovement(false, Enums.Side.None);
            }
            else
            {
                if (bot.RelativeX > bot.BotPatrolX)
                {
                    bot.SetMovement(true, Enums.Side.Left);
                }

                if (bot.RelativeX < bot.BotPatrolX)
                {
                    bot.SetMovement(true, Enums.Side.Right);
                }

                if ((int)bot.RelativeX == (int)bot.BotPatrolX)
                {
                    bot.SetMovement(false, Enums.Side.None);
                    bot.BotPatrolX = 0;
                    bot.BotPatrolWait = 5;
                }
            }
        }

        private void GoTo(Sprite bot)
        {
            if (bot.RelativeX > bot.BotPatrolX_Area)
            {
                bot.SetMovement(true, Enums.Side.Left);
            }

            if (bot.RelativeX < bot.BotPatrolX_Area)
            {
                bot.SetMovement(true, Enums.Side.Right);
            }

            if ((int)bot.RelativeX == (int)bot.BotPatrolX_Area)
            {
                bot.SetMovement(false, Enums.Side.None);
                bot.BotGoTo = false;
            }
        }
    }
}
