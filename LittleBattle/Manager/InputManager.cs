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

    public static bool p2_jump_key_pressed = false;
    public static bool p2_attack_key_pressed = false;

    public static bool visibleCamerman = false;

    private static int botID = 9;
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

    public static void Update(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects, Resolution resolution, KeyMappingsManager keyMappings)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        KeyboardState keyboard = Keyboard.GetState();
        var gamepad = GamePad.GetState(PlayerIndex.One);

        if (players[0].spriteType == SpriteType.Player1) 
        { 
            if (gamepad.IsConnected)
            {
                Player1_Gamepad(gamepad, players[0]);
            }
            else
            {
                Player1_Keybord(players, bots, objects, keyboard, players[0], keyMappings);
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
        DebugCommand(players, bots, objects);
    }

    private static void Player1_Keybord(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects, KeyboardState keyboard, Sprite player, KeyMappingsManager keyMappings)
    {    
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
            player.SetInteractionObjects(players, bots, objects);
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

    private static void Player1_Gamepad(GamePadState gamepad, Sprite player)
    {
        if (gamepad.DPad.Left == ButtonState.Pressed)
        {
            player.SetMovement(true, Side.Left);
        }
        else if (gamepad.DPad.Right == ButtonState.Pressed)
        {
            player.SetMovement(true, Side.Right);
        }

        if (gamepad.DPad.Left == ButtonState.Released && gamepad.DPad.Right == ButtonState.Released)
        {
            player.SetMovement(false, Side.None);
        }

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
    }

    private static void Player2_Keybord(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects, KeyboardState keyboard, Sprite player)
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
            player.SetInteractionObjects(players, bots, objects);
        }
    }

    public static void UpdateResolution(Resolution resolution)
    {
        //_lastKeyboard = _currentKeyboard;
        //_currentKeyboard = Keyboard.GetState();
        if (IsKeyPressed(Keys.F1)) resolution.SetResolution(new Size(600, 400));
        if (IsKeyPressed(Keys.F2)) resolution.SetResolution(new Size(800, 600));
        if (IsKeyPressed(Keys.F3)) resolution.SetResolution(new Size(1280, 720));
        if (IsKeyPressed(Keys.F4)) resolution.SetResolution(new Size(1920, 1080));
        //if (InputManager.IsKeyPressed(Keys.F4)) SetBorderlessScreen();
        if (IsKeyPressed(Keys.F5)) { 
            resolution.SetFullScreen(); 
        }
    }

    //Debug
    public static void DebugCommand(List<Sprite> players, List<SpriteBot> bots, List<SpriteObject> objects)
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
            botID++;
            var val = Globals.NegativeLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(botID, new Vector2(val, 0), SpriteType.Bot, Team.Team1, ClassType.Warrior));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
        }
        if (IsKeyPressed(Keys.D7))
        {
            botID++;
            var val = Globals.NegativeLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(botID, new Vector2(val, 0), SpriteType.Bot, Team.Team1, ClassType.Archer));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
        }

        if (IsKeyPressed(Keys.D9))
        {
            botID++;
            var val = Globals.PositiveLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(botID, new Vector2(val, 0), SpriteType.Bot, Team.Team2, ClassType.Warrior));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
        }
        if (IsKeyPressed(Keys.D8))
        {
            botID++;
            var val = Globals.PositiveLimit.Width + Globals.GroundX;
            bots.Add(new SpriteBot(botID, new Vector2(val, 0), SpriteType.Bot, Team.Team2, ClassType.Archer));
            var bot = bots[bots.Count - 1];
            bot.CenterX_Adjust();
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

        if (IsKeyPressed(Keys.T))
        {
            players[0].SetObject(SpriteType.ArcherTower);
        }
        if (IsKeyPressed(Keys.Y))
        {
            //players[1].SetObject(SpriteType.ArcherTower);
            objects.Add(new SpriteObject(null, Side.None, SpriteType.Resource, players[0].Position));
        }
        if (IsKeyPressed(Keys.U))
        {
            objects.Add(new SpriteObject(null, Side.None, SpriteType.Tree01, players[0].Position));
        }

        if (IsKeyPressed(Keys.F8))
        {
            Globals.DebugArea = !Globals.DebugArea;
        }
    }
}
