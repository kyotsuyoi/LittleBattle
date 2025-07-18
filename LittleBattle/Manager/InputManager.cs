using LittleBattle.Classes;
using LittleBattle.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LittleBattle.Classes.Enums;

public static class InputManager
{
    private static KeyboardState _lastKeyboard;
    private static KeyboardState _currentKeyboard;

    public static bool p1_jump_key_pressed = false;
    public static bool p1_attack_key_pressed = false;

    public static bool p1_interact_key_pressed = false;
    public static bool p1_action_key_pressed = false;
    public static bool p1_next_key_pressed = false;
    public static bool p1_prev_key_pressed = false;

    public static bool p2_jump_key_pressed = false;
    public static bool p2_attack_key_pressed = false;

    public static bool visibleCamerman = false;

    public static bool ClearBot = false;
    public static bool CommandBot = false;

    public static bool IsKeyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static bool IsKeyHold(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static void Update(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects, List<SpriteObjectItem> objectItems, Sprite cameraman, Resolution resolution, Canvas _canvas, KeyMappingsManager keyMappings, GraphicsDeviceManager graphics)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        KeyboardState keyboard = Keyboard.GetState();
        var gamepad = GamePad.GetState(PlayerIndex.One);

        if (players[0].spriteType == SpriteType.Player1) 
        { 
            if (gamepad.IsConnected)
            {
                Player1_Gamepad(gamepad, players, objects, objectItems, cameraman);
            }
            else
            {
                Player1_Keybord(players, objects, objectItems, keyboard, keyMappings);
            }
        }

        //if (players[1].spriteType == SpriteType.Player2)
        //{
        //    if (false /*gamepad.IsConnected*/)
        //    {
        //        Player1_Gamepad(gamepad, players[1]);
        //    }
        //    else
        //    {
        //        Player2_Keybord(players, bots, objects, keyboard, players[1]);
        //    }
        //}

        UpdateResolution(resolution);
        DebugCommand(players, bots, objects, _canvas, graphics);
    }

    private static void Player1_Keybord(List<Sprite> players, List<SpriteObject> objects, List<SpriteObjectItem> objectItems, KeyboardState keyboard, KeyMappingsManager keyMappings)
    {
        var player = players[0];
        if (keyboard.IsKeyDown(keyMappings.MoveLeft))
        {
            player.SetMovement(true, Side.Left);
        }
        else if (keyboard.IsKeyDown(keyMappings.MoveRight))
        {
            player.SetMovement(true, Side.Right);
        }

        if (keyboard.IsKeyDown(keyMappings.MoveUp))
        {
            player.SetMovement(false, Side.Up);
        }
        else if (keyboard.IsKeyDown(keyMappings.MoveDown))
        {
            player.SetMovement(false, Side.Down);
        }

        if (keyboard.IsKeyUp(keyMappings.MoveLeft) && keyboard.IsKeyUp(keyMappings.MoveRight)
            && keyboard.IsKeyUp(keyMappings.MoveUp) && keyboard.IsKeyUp(keyMappings.MoveDown))
        {
            player.SetMovement(false, Side.None);
        }

        if (keyboard.IsKeyDown(keyMappings.Jump) && !p1_jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            p1_jump_key_pressed = true;
            player.SetJump();
        }

        if (keyboard.IsKeyUp(keyMappings.Jump))
        {
            p1_jump_key_pressed = false;
        }
        

        if (keyboard.IsKeyDown(keyMappings.Attack) && !p1_attack_key_pressed)
        {
            p1_attack_key_pressed = true;
            player.SetAttack();
        }

        if (keyboard.IsKeyUp(keyMappings.Attack))
        {
            p1_attack_key_pressed = false;
        }

        if (IsKeyPressed(Keys.K))
        {
            player.SetInteractionObjects(objects, objectItems);
        }

        if (IsKeyPressed(Keys.Q))
        {
            player.PreviousAction();
        }

        if (IsKeyPressed(Keys.E))
        {
            player.NextAction();
        }

        if (IsKeyPressed(Keys.J))
        {
            player.ActionExecute();
        }
    }

    private static void Player1_Gamepad(GamePadState gamepad, List<Sprite> players, List<SpriteObject> objects, List<SpriteObjectItem> objectItems, Sprite cameraman)
    {
        var player = players[0];
        //if (gamepad.DPad.Left == ButtonState.Pressed)
        //{
        //    player.SetMovement(true, Side.Left);
        //}
        //else if (gamepad.DPad.Right == ButtonState.Pressed)
        //{
        //    player.SetMovement(true, Side.Right);
        //}


        if (gamepad.ThumbSticks.Left.Y < 0)
        {
            player.SetMovement(false, Side.Down, -gamepad.ThumbSticks.Left.Y);
        }
        else if (gamepad.ThumbSticks.Left.Y > 0)
        {
            player.SetMovement(false, Side.Up, gamepad.ThumbSticks.Left.Y);
        }

        if (gamepad.ThumbSticks.Left.X > 0)
        {
            player.SetMovement(true, Side.Right, gamepad.ThumbSticks.Left.X);
        }
        else if(gamepad.ThumbSticks.Left.X < 0)
        {
            player.SetMovement(true, Side.Left, -gamepad.ThumbSticks.Left.X);
        }

        if (gamepad.ThumbSticks.Left.X == 0 && gamepad.ThumbSticks.Left.Y == 0)
        {
            player.SetMovement(false, Side.None);
        }

        if (gamepad.ThumbSticks.Right.X > 0)
        {
            cameraman.SetMovement(true, Side.Right, gamepad.ThumbSticks.Right.X*2);
        }
        else if (gamepad.ThumbSticks.Right.X < 0)
        {
            cameraman.SetMovement(true, Side.Left, -gamepad.ThumbSticks.Right.X*2);
        }

        if (gamepad.ThumbSticks.Right.X == 0 && gamepad.ThumbSticks.Right.Y == 0)
        {
            //cameraman.Attribute.Speed = 2;
        }

        //if (gamepad.DPad.Up == ButtonState.Pressed)
        //{
        //    player.SetMovement(false, Side.Up);
        //}
        //else if (gamepad.DPad.Down == ButtonState.Pressed)
        //{
        //    player.SetMovement(false, Side.Down);
        //}

        //if (gamepad.DPad.Left == ButtonState.Released && gamepad.DPad.Right == ButtonState.Released &&
        //    gamepad.DPad.Up == ButtonState.Released && gamepad.DPad.Down == ButtonState.Released)
        //{
        //    player.SetMovement(false, Side.None);
        //}

        if (gamepad.Buttons.A == ButtonState.Pressed && !p1_jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            p1_jump_key_pressed = true;
            player.SetJump();
        }

        if (gamepad.Buttons.A == ButtonState.Released)
        {
            p1_jump_key_pressed = false;
        }

        if (gamepad.Buttons.X == ButtonState.Pressed && !p1_attack_key_pressed)
        {
            p1_attack_key_pressed = true;
            player.SetAttack();
        }

        if (gamepad.Buttons.X == ButtonState.Released)
        {
            p1_attack_key_pressed = false;
        }

        if (gamepad.Buttons.Y == ButtonState.Pressed && !p1_interact_key_pressed)
        {
            p1_interact_key_pressed = true;
            player.SetInteractionObjects(objects, objectItems);
        }
        if (gamepad.Buttons.Y == ButtonState.Released)
        {
            p1_interact_key_pressed = false;
        }

        if (gamepad.Buttons.B == ButtonState.Pressed && !p1_action_key_pressed)
        {
            p1_action_key_pressed = true;
            player.ActionExecute();
        }
        if (gamepad.Buttons.B == ButtonState.Released)
        {
            p1_action_key_pressed = false;
        }

        if (gamepad.Buttons.LeftShoulder == ButtonState.Pressed && !p1_prev_key_pressed)
        {
            p1_prev_key_pressed = true;
            player.PreviousAction();
        }
        if (gamepad.Buttons.LeftShoulder == ButtonState.Released)
        {
            p1_prev_key_pressed = false;
        }

        if (gamepad.Buttons.RightShoulder == ButtonState.Pressed && !p1_next_key_pressed)
        {
            p1_next_key_pressed = true;
            player.NextAction();
        }
        if (gamepad.Buttons.RightShoulder == ButtonState.Released)
        {
            p1_next_key_pressed = false;
        }
    }

    private static void Player2_Keybord(List<SpriteObject> objects, List<SpriteObjectItem> objectItems, KeyboardState keyboard, Sprite player)
    {
        if (keyboard.IsKeyDown(Keys.Left))
        {
            player.SetMovement(true, Side.Left);
        }
        else if (keyboard.IsKeyDown(Keys.Right))
        {
            player.SetMovement(true, Side.Right);
        }

        if (keyboard.IsKeyDown(Keys.Up))
        {
            player.SetMovement(false, Side.Up);
        }
        else if (keyboard.IsKeyDown(Keys.Down))
        {
            player.SetMovement(false, Side.Down);
        }

        if (keyboard.IsKeyUp(Keys.Left) && keyboard.IsKeyUp(Keys.Right)
            && keyboard.IsKeyUp(Keys.Up) && keyboard.IsKeyUp(Keys.Down))
        {
            player.SetMovement(false, Side.None);
        }

        if (keyboard.IsKeyDown(Keys.NumPad0) && !p2_jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            p2_jump_key_pressed = true;
            player.SetJump();
        }

        if (keyboard.IsKeyUp(Keys.NumPad0) )
        {
            p2_jump_key_pressed = false;
        }

        if (keyboard.IsKeyDown(Keys.NumPad1) && !p2_attack_key_pressed)
        {
            p2_attack_key_pressed = true;
            player.SetAttack();
        }

        if (keyboard.IsKeyUp(Keys.NumPad1))
        {
            p2_attack_key_pressed = false;
        }

        if (IsKeyPressed(Keys.NumPad4))
        {
            player.SetInteractionObjects(objects, objectItems);
        }
    }

    public static void UpdateResolution(Resolution resolution)
    {
        //_lastKeyboard = _currentKeyboard;
        //_currentKeyboard = Keyboard.GetState();
        //if (IsKeyPressed(Keys.F1)) resolution.SetResolution(new Size(600, 400));
        if (IsKeyPressed(Keys.F2)) resolution.SetResolution(new Size(800, 600));
        if (IsKeyPressed(Keys.F3)) resolution.SetResolution(new Size(1280, 720));
        if (IsKeyPressed(Keys.F4)) resolution.SetResolution(new Size(1920, 1080));
        if (InputManager.IsKeyPressed(Keys.F1)) resolution.SetBorderlessScreen();
        if (IsKeyPressed(Keys.F5)) { 
            resolution.SetFullScreen(); 
        }
    }

    //Debug
    public static void DebugCommand(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects, Canvas _canvas, GraphicsDeviceManager graphics)
    {
        if (IsKeyPressed(Keys.F9))
        {
            Globals.Debug = !Globals.Debug;
        }

        if (!Globals.Debug) return;

        if (IsKeyPressed(Keys.F6))
        {
            if (visibleCamerman)
            {
                visibleCamerman = false;
                return;
            }
            visibleCamerman = true;
        }

        if (IsKeyPressed(Keys.P))
        {
            foreach (var player in players)
            {
                player.Revive();
            }
        }

        if (IsKeyPressed(Keys.O))
        {
            foreach (var bot in bots)
            {
                bot.Revive();
            }
        }

        if (IsKeyPressed(Keys.D6))
        {
            var val = Globals.NegativeLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(new Vector2(val, Globals.GroundLevel), SpriteType.Bot, Team.Team1, ClassType.Warrior, graphics));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
            bot.GoTo = true;
            bot.PatrolX_Area = players[0].RelativeX;
        }
        if (IsKeyPressed(Keys.D7))
        {
            var val = Globals.NegativeLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(new Vector2(val, Globals.GroundLevel), SpriteType.Bot, Team.Team1, ClassType.Archer, graphics));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
            bot.GoTo = true;
            bot.PatrolX_Area = players[0].RelativeX;
        }


        if (IsKeyPressed(Keys.D8))
        {
            var val = Globals.PositiveLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(new Vector2(val, Globals.GroundLevel), SpriteType.Bot, Team.Team2, ClassType.Archer, graphics));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
            bot.GoTo = true;
            bot.PatrolX_Area = 0;
        }

        if (IsKeyPressed(Keys.D9))
        {
            var val = Globals.PositiveLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(new Vector2(val, Globals.GroundLevel), SpriteType.Bot, Team.Team2, ClassType.Warrior, graphics));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
            bot.GoTo = true;
            bot.PatrolX_Area = 0;
        }

        if (IsKeyPressed(Keys.D0))
        {
            ClearBot = true;
        }

        if (IsKeyPressed(Keys.N))
        {
            CommandBot = true;
        }

        if (IsKeyPressed(Keys.B))
        {
            var alliedBots = players.Where(bot => players[0].Team == bot.Team).ToList();
            foreach (var bot in bots)
            {
                bot.Patrol = !bot.Patrol;
                if (!bot.Patrol && !bot.GoTo)
                {
                    bot.SetMovement(false, bot.GetSide());
                    bot.PatrolWait = 0;
                    bot.Patrol = false;
                }
            }
        }

        //Call nearby bots to the player's position
        //Need to migrate this code to BotManager
        if (IsKeyPressed(Keys.V))
        {
            var alliedBots = players.Where(bot => players[0].Team == bot.Team).ToList();
            foreach (var bot in bots)
            {
                bool isRenge = false;
                if (bot.RelativeX < players[0].RelativeX)
                {
                    if ((players[0].RelativeX - bot.RelativeX < 400)
                        && (bot.RelativeX < players[0].RelativeX))
                    {
                        isRenge = true;
                    }
                }

                if (bot.RelativeX > players[0].RelativeX)
                {
                    if ((bot.RelativeX - (players[0].RelativeX) < 400)
                        && (players[0].RelativeX) * 1 < bot.RelativeX)
                    {
                        isRenge = true;
                    }
                }

                if (isRenge)
                {
                    bot.PatrolX_Area = players[0].RelativeX;
                    bot.GoTo = true;
                }

            }
        }

        if (IsKeyPressed(Keys.Z))
        {
            players[0].AddBagItem(SpriteType.Wood, 10);
            players[0].AddBagItem(SpriteType.Vine, 10);
            players[0].AddBagItem(SpriteType.Fruit, 10);
            players[0].AddBagItem(SpriteType.Iron, 10);
        }

        if (IsKeyPressed(Keys.T))
        {
            players[0].SetObject(SpriteType.ArcherTower);
        }

        if (IsKeyPressed(Keys.Y))
        {
            //players[1].SetObject(SpriteType.ArcherTower);
            objects.Add(new SpriteObject(null, Side.Right, SpriteType.ResourceIron, players[0].Position, graphics));
        }

        if (IsKeyPressed(Keys.U))
        {
            objects.Add(new SpriteObject(null, Side.Right, SpriteType.Tree01, players[0].Position, graphics));
        }

        if (IsKeyPressed(Keys.F8))
        {
            Globals.DebugArea = !Globals.DebugArea;
        }

        if (IsKeyPressed(Keys.Left))
        {
            _canvas.adjustX++;
            _canvas.SetDestinationRectangle();
        }
        if (IsKeyPressed(Keys.Right))
        {
            _canvas.adjustX--;
            _canvas.SetDestinationRectangle();
        }
        if (IsKeyPressed(Keys.Up))
        {
            _canvas.adjustY++;
            _canvas.SetDestinationRectangle();
        }
        if (IsKeyPressed(Keys.Down))
        {
            _canvas.adjustY--;
            _canvas.SetDestinationRectangle();
        }
    }
}
