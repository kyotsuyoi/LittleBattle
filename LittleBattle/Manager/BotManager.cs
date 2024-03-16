using LittleBattle.Classes;
using LittleBattle.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

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

        public void Update(List<SpriteBot> bots, List<Sprite> players, List<SpriteObjectItem> objectItems, bool goToCommand = false)
        {
            var aliveBots = bots.Where(bot => !bot.IsDead()).ToList();
            var test = bots.Where(bot => bot.Team != Enums.Team.Team1).ToList();

            foreach (var bot in aliveBots)
            {
                var target_enemy = SetTarget_Enemy(bot, players, bots);
                if (target_enemy != null){
                    if (Target_EnemyAttack(bot, target_enemy)) goto _continue;

                    if (!Target_EnemyDistance(bot, target_enemy) || target_enemy == null)
                    {
                        if (goToCommand && bot.Team == players[0].Team)
                        {
                            bot.PatrolX_Area = players[0].RelativeX;
                            bot.GoTo = true;
                        }
                        else if (bot.Team != players[0].Team)
                        {
                            bot.Patrol = true;
                        }

                        if (bot.GoTo)
                        {
                            GoTo(bot);
                        }
                        else
                        {
                            Patrol(bot);
                        }
                    }
                }

                var target_objectItem = SetTarget_ObjectItem(bot, objectItems);
                if (target_objectItem != null)
                {
                    if (Target_GetObjectItem(bot, target_objectItem, objectItems)) goto _continue;

                    if (!Target_ObjectItemDistance(bot, target_objectItem) || target_objectItem == null)
                    {
                        if (goToCommand && bot.Team == players[0].Team)
                        {
                            bot.PatrolX_Area = players[0].RelativeX;
                            bot.GoTo = true;
                        }
                        else if (bot.Team != players[0].Team)
                        {
                            bot.Patrol = true;
                        }

                        if (bot.GoTo)
                        {
                            GoTo(bot);
                        }
                        else
                        {
                            Patrol(bot);
                        }
                    }
                }
                else
                {
                    bot.SetMovement(false, bot.GetSide());
                }
                
                _continue:;
            }
        }

        public void UpdateCamerman(SpriteCameraman Cameraman, List<Sprite> players)
        {
            var stop = false;
            if (players[0].Climb) {
                stop = true;
            }
            var player_position_side = players[0].Position;
            if (Cameraman.GetSide() != players[0].GetSide())
            {
                //Cameraman.Attribute.Speed = 3;
                lastSecond = Globals.TotalSeconds - 1f;
            }

            if (players[0].GetSide() == Enums.Side.Right)
            {
                player_position_side *= new Vector2(1f, 1);
            }
            else
            if (players[0].GetSide() == Enums.Side.Left)
            {
                player_position_side /= new Vector2(1f, 1);
            }

            //if (players[0].GetSide() == Enums.Side.Right)
            //{
            //    if (players[0].Position.X - Globals.Size.Width / 2 <= Cameraman.Position.X + Cameraman.Size.X / 2)
            //    {
            //        stop = true;
            //    }
            //    if ((Cameraman.Position.X > player_position_side.X) || Cameraman.RelativeX + Globals.Size.Width / 2 >= Globals.PositiveLimit.Width)
            //    {
            //        stop = true;
            //    }
            //}
            //else if (players[0].GetSide() == Enums.Side.Left)
            //{
            //    if (players[0].RelativeX - Globals.Size.Width / 2 > Cameraman.RelativeX)
            //    {
            //        stop = true;
            //    }
            //    if ((Cameraman.Position.X < player_position_side.X) || Cameraman.RelativeX - Globals.Size.Width / 2 <= Globals.NegativeLimit.Width)
            //    {
            //        stop = true;
            //    }
            //}

            Collision collision = new Collision();

            var m = 1.2f;
            var d = 0.8f;
            if (Cameraman.classType == Enums.ClassType.None)
            {
                m = 0.0001f;
                d = 0.0001f;
            }

            var collide = collision.SquareCollision(
                new Vector2((int)Cameraman.RelativeX * d, 0),
                Cameraman.GetSize() * new Vector2(m, 1),
                new Vector2((int)players[0].RelativeX * d, 0),
                players[0].GetSize() * new Vector2(m, 1)
            );

            Cameraman.SameSpeed = false;
            if (collide)
            {
                stop = true;
                Cameraman.SameSpeed = true;
                Cameraman.Attribute.Speed = players[0].Attribute.CurrentSpeed*2;
            }

            if (!Cameraman.SameSpeed)
            {
                Cameraman.Attribute.Speed = 4;
            }

            if (stop)
            {
                if (players[0].Attribute.CurrentSpeed <= 0)
                {
                    Cameraman.SetMovement(false, Enums.Side.None);
                    Cameraman.Attribute.Speed = 2;
                    return;
                }
            }

            if (Globals.TotalSeconds - lastSecond < 0.1f)
            {
                return;
            }

            lastSecond = Globals.TotalSeconds;

            //if ((int)Cameraman.Attribute.Speed == (int)players[0].Attribute.Speed)
            //{
            //    Cameraman.Attribute.Speed = players[0].Attribute.Speed-0.2f;
            //}

            //if (players[0].GetSide() == Enums.Side.Left)
            //{
            //    if (Cameraman.Position.X < player_position_side.X && players[0].Walk)
            //    {
            //        Cameraman.Attribute.Speed = players[0].Attribute.Speed;
            //    }
            //    else
            //    {
            //        Cameraman.Attribute.Speed += 0.1f;
            //    }
            //    Cameraman.SetMovement(true, Enums.Side.Left);
            //} else 
            //if (players[0].GetSide() == Enums.Side.Right)
            //{                
            //    if (Cameraman.Position.X > player_position_side.X && players[0].Walk) {
            //        Cameraman.Attribute.Speed = players[0].Attribute.Speed;
            //    }
            //    else
            //    {
            //        Cameraman.Attribute.Speed += 0.1f;
            //    }
            //    Cameraman.SetMovement(true, Enums.Side.Right);
            //}

            if (Cameraman.Position.X < player_position_side.X)
            {
                //Cameraman.Attribute.Speed = players[0].Attribute.Speed;
                Cameraman.SetMovement(true, Enums.Side.Right);
            }

            if (Cameraman.Position.X > player_position_side.X)
            {
                //Cameraman.Attribute.Speed = players[0].Attribute.Speed;
                Cameraman.SetMovement(true, Enums.Side.Left);
            }
        }

        private Sprite SetTarget_Enemy(SpriteBot bot, List<Sprite> players, List<SpriteBot> bots)
        {
            Sprite target = null;
            var alivePlayers = players.Where(player => !player.IsDead() && player.Team != bot.Team).ToList();
            var aliveBots_enemy = bots.Where(enemyBot => !enemyBot.IsDead() && enemyBot.Team != bot.Team).ToList();
            var allEnemies = alivePlayers;

            foreach (var _bot in aliveBots_enemy)
            {
                allEnemies.Add(_bot);
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

        private SpriteObjectItem SetTarget_ObjectItem(SpriteBot bot, List<SpriteObjectItem> spriteObjectItem)
        {
            SpriteObjectItem target = null;

            if (spriteObjectItem.Count() <= 0) return target;
            target = spriteObjectItem[0];
            float distance = 0;

            if (bot.RelativeX >= target.RelativeX)
            {
                distance = bot.RelativeX - target.RelativeX;
            }
            if (bot.RelativeX < target.RelativeX)
            {
                distance = target.RelativeX - bot.RelativeX;
            }

            foreach (var item in spriteObjectItem)
            {
                float inner_distace = 0;
                if (bot.RelativeX >= item.RelativeX)
                {
                    inner_distace = bot.RelativeX - item.RelativeX;
                }
                if (bot.RelativeX < item.RelativeX)
                {
                    inner_distace = item.RelativeX - bot.RelativeX;
                }
                if (inner_distace < distance)
                {
                    distance = inner_distace;
                    target = item;
                }
            }
            return target;
        }

        private bool Target_EnemyDistance(Sprite bot, Sprite target)
        {
            bool range = false;
            if (target == null) return false;

            if (bot.RelativeX < target.RelativeX)
            {
                if ((target.RelativeX - bot.RelativeX < 400)
                    && (bot.RelativeX < target.RelativeX))
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if (bot.RelativeX + bot.GetSize().X > target.RelativeX)
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

            if (bot.RelativeX + bot.GetSize().X >= target.RelativeX)
            {
                if ((bot.RelativeX - (target.RelativeX) < 400)
                    && (target.RelativeX) * 1 < bot.RelativeX)
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if (bot.RelativeX - bot.GetSize().X - bot.Attribute.Range < target.RelativeX)
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

        private bool Target_ObjectItemDistance(Sprite bot, SpriteObjectItem target)
        {

            if (bot.classType != Enums.ClassType.Newbie) return false;

            bool renge = false;
            if (target == null) return false;

            if (bot.RelativeX < target.RelativeX)
            {
                if ((target.RelativeX - bot.RelativeX < 400)
                    && (bot.RelativeX < target.RelativeX))
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if (bot.RelativeX > target.RelativeX)
                    {
                        bot.SetMovement(false, Enums.Side.Right);
                    }
                    else
                    {
                        bot.SetMovement(true, Enums.Side.Right);
                    }
                    renge = true;
                }
            }

            if (bot.RelativeX > target.RelativeX)
            {
                if ((bot.RelativeX - (target.RelativeX) < 400)
                    && (target.RelativeX) * 1 < bot.RelativeX)
                {
                    //Needs to get Range using a method (calc Attack Range)
                    if (bot.RelativeX < target.RelativeX)
                    {
                        bot.SetMovement(false, Enums.Side.Left);
                    }
                    else
                    {
                        bot.SetMovement(true, Enums.Side.Left);
                    }
                    renge = true;
                }
            }

            return renge;
        }

        private bool Target_EnemyAttack(Sprite bot, Sprite player)
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
                bot.GetSize() * new Vector2(m, 1),
                new Vector2((int)player.RelativeX * d, 0),
                player.GetSize() * new Vector2(m, 1)
            );

            if (collide)
            {
                if (bot.RelativeX > player.RelativeX) bot.SetMovement(false, Enums.Side.Left);
                if (bot.RelativeX < player.RelativeX) bot.SetMovement(false, Enums.Side.Right);
                bot.SetAttack();
            }
            return collide;
        }

        private bool Target_GetObjectItem(Sprite bot, SpriteObjectItem item, List<SpriteObjectItem> objectItems)
        {
            Collision collision = new Collision();

            //var m = 0.5f;
            //var d = 0.5f;

            //var collide = collision.SquareCollision(
            //    new Vector2((int)bot.RelativeX * d, 0),
            //    bot.GetSize() * new Vector2(m, 1),
            //    new Vector2((int)item.RelativeX * d, 0),
            //    item.GetSize() * new Vector2(m, 1)
            //);

            var isCollide = collision.IsCollide(bot.GetRectangle(), item.GetRectangle());

            if (isCollide)
            {
                if (bot.RelativeX > item.RelativeX) bot.SetMovement(false, Enums.Side.Left);
                if (bot.RelativeX < item.RelativeX) bot.SetMovement(false, Enums.Side.Right);
                bot.SetInteractionObjects_HoldDown(objectItems);
                bot.UseSetBagWorker();
            }
            return isCollide;
        }

        private void Patrol(SpriteBot bot)
        {
            if (!bot.Patrol) return;
            if (bot.PatrolWait > 0)
            {
                bot.PatrolWait -= Globals.ElapsedSeconds;
                return;
            }

            if (bot.PatrolX == 0)
            {
                int randomVal;
                Random random = new Random();
                randomVal = random.Next(400) * 1 - 200;
                bot.PatrolX = randomVal;
                bot.SetMovement(false, Enums.Side.None);
                
                bot.PatrolX = randomVal;
                bot.SetMovement(false, Enums.Side.None);
            }
            else
            {
                if (bot.RelativeX > bot.PatrolX)
                {
                    bot.SetMovement(true, Enums.Side.Left);
                }

                if (bot.RelativeX < bot.PatrolX)
                {
                    bot.SetMovement(true, Enums.Side.Right);
                }

                if ((int)bot.RelativeX == (int)bot.PatrolX)
                {
                    bot.SetMovement(false, Enums.Side.None);
                    bot.PatrolX = 0;
                    bot.PatrolWait = 5;
                }
            }
        }

        private void GoTo(SpriteBot bot)
        {
            if (bot.RelativeX > bot.PatrolX_Area)
            {
                bot.SetMovement(true, Enums.Side.Left);
            }

            if (bot.RelativeX < bot.PatrolX_Area)
            {
                bot.SetMovement(true, Enums.Side.Right);
            }

            if ((int)bot.RelativeX == (int)bot.PatrolX_Area)
            {
                bot.SetMovement(false, Enums.Side.None);
                bot.GoTo = false;
            }
        }

        private void VerifyToolBags(Sprite bot)
        {

        }
    }
}
