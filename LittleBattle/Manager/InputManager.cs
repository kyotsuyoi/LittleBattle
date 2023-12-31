using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

public static class InputManager
{
    private static KeyboardState _lastKeyboard;
    private static KeyboardState _currentKeyboard;

    public static bool p1_jump_key_pressed = false;
    public static bool p1_attack_key_pressed = false;

    public static bool p2_jump_key_pressed = false;
    public static bool p2_attack_key_pressed = false;

    public static bool IsKeyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static bool IsKeyHold(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static void Update(Sprite player, List<Sprite> bots)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        KeyboardState keyboard = Keyboard.GetState();
        var gamepad = GamePad.GetState(PlayerIndex.One);

        if (player.spriteType == Enums.SpriteType.Player1) 
        { 
            if (gamepad.IsConnected)
            {
                Player1_Gamepad(gamepad, player);
            }
            else
            {
                Player1_Keybord(keyboard, player);
            }
        }

        if (player.spriteType == Enums.SpriteType.Player2)
        {
            if (false /*gamepad.IsConnected*/)
            {
                Player1_Gamepad(gamepad, player);
            }
            else
            {
                Player2_Keybord(keyboard, player);
            }
        }

        DebugCommand(keyboard, player, bots);
    }

    private static void Player1_Keybord(KeyboardState keyboard, Sprite player)
    {
        if (keyboard.IsKeyDown(Keys.A))
        {
            //player.SetMovement(Enums.Direction.WalkLeft);
            player.SetMovement(true, Enums.Side.Left);
        }
        else if (keyboard.IsKeyDown(Keys.D))
        {
            //player.SetMovement(Enums.Direction.WalkRight);
            player.SetMovement(true, Enums.Side.Right);
        }

        if (keyboard.IsKeyUp(Keys.A) && keyboard.IsKeyUp(Keys.D))
        {
            //player.Walk = false;
            player.SetMovement(false, Enums.Side.None);
        }

        if (keyboard.IsKeyDown(Keys.Space) && !p1_jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            p1_jump_key_pressed = true;
            player.SetJump();
        }

        if (keyboard.IsKeyUp(Keys.Space))
        {
            p1_jump_key_pressed = false;
        }

        if (keyboard.IsKeyDown(Keys.M) && !p1_attack_key_pressed)
        {
            p1_attack_key_pressed = true;
            player.SetAttack();
        }

        if (keyboard.IsKeyUp(Keys.M))
        {
            p1_attack_key_pressed = false;
        }
    }

    private static void Player1_Gamepad(GamePadState gamepad, Sprite player)
    {
        if (gamepad.DPad.Left == ButtonState.Pressed)
        {
            //player.SetMovement(Enums.Direction.WalkLeft);
            player.SetMovement(true, Enums.Side.Left);
        }
        else if (gamepad.DPad.Right == ButtonState.Pressed)
        {
            //player.SetMovement(Enums.Direction.WalkRight);
            player.SetMovement(true, Enums.Side.Right);
        }

        if (gamepad.DPad.Left == ButtonState.Released && gamepad.DPad.Right == ButtonState.Released)
        {
            //player.Walk = false;
            player.SetMovement(false, Enums.Side.None);
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

    private static void DebugCommand(KeyboardState keyboard, Sprite player, List<Sprite> bots)
    {
        if (keyboard.IsKeyDown(Keys.P)) { 
                player.Revive();            
        }

        if (keyboard.IsKeyDown(Keys.O))
        {
            foreach (var bot in bots)
            {
                bot.Revive();
            }
        }
    }

    private static void Player2_Keybord(KeyboardState keyboard, Sprite player)
    {
        if (keyboard.IsKeyDown(Keys.Left))
        {
            //player.SetMovement(Enums.Direction.WalkLeft);
        }
        else if (keyboard.IsKeyDown(Keys.Right))
        {
            //player.SetMovement(Enums.Direction.WalkRight);
        }

        if (keyboard.IsKeyUp(Keys.Left) && keyboard.IsKeyUp(Keys.Right))
        {
            player.Walk = false;
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
    }

    public static void UpdateResolution(Resolution resolution)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        KeyboardState keyboard = Keyboard.GetState();
        if (IsKeyPressed(Keys.F1)) resolution.SetResolution(new Size(600, 400));
        if (IsKeyPressed(Keys.F2)) resolution.SetResolution(new Size(800, 600));
        if (IsKeyPressed(Keys.F3)) resolution.SetResolution(new Size(1280, 720));
        if (IsKeyPressed(Keys.F4)) resolution.SetResolution(new Size(1920, 1080));
        //if (InputManager.IsKeyPressed(Keys.F4)) SetBorderlessScreen();
        if (IsKeyPressed(Keys.F5)) { 
            resolution.SetFullScreen(); 
        }
    }
}
