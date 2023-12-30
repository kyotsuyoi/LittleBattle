using LittleBattle.Classes;
using Microsoft.Xna.Framework.Input;
using System.Drawing;

public static class InputManager
{
    private static KeyboardState _lastKeyboard;
    private static KeyboardState _currentKeyboard;
    public static bool jump_key_pressed = false;
    public static bool attack_key_pressed = false;

    public static bool IsKeyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static bool IsKeyHold(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
    }

    public static void Update(Sprite player)
    {
        _lastKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
        KeyboardState keyboard = Keyboard.GetState();
        if(player.spriteType == Enums.SpriteType.Player1) Player1(keyboard, player);
        if(player.spriteType == Enums.SpriteType.Player2) Player2(keyboard, player);
    }

    private static void Player1(KeyboardState keyboard, Sprite player)
    {
        if (keyboard.IsKeyDown(Keys.A))
        {
            player.Direction = Enums.Direction.WalkLeft;
            player.Walk = true;
        }
        else if (keyboard.IsKeyDown(Keys.D))
        {
            player.Direction = Enums.Direction.WalkRight;
            player.Walk = true;
        }

        if (keyboard.IsKeyUp(Keys.A) && keyboard.IsKeyUp(Keys.D))
        {
            player.Walk = false;
        }

        //if (keyboard.IsKeyDown(Keys.W) && player.Position.Y > 0)
        //{
        //    player.Position = new Vector2(player.Position.X, 200);
        //}

        if (keyboard.IsKeyDown(Keys.Space) && !jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            jump_key_pressed = true;
            player.Jump = true;
        }

        if (keyboard.IsKeyUp(Keys.Space))
        {
            jump_key_pressed = false;
        }

        if (keyboard.IsKeyDown(Keys.M) && !attack_key_pressed)
        {
            attack_key_pressed = true;
            player.SetAttack();
        }

        if (keyboard.IsKeyUp(Keys.M))
        {
            attack_key_pressed = false;
        }
    }

    private static void Player2(KeyboardState keyboard, Sprite player)
    {
        if (keyboard.IsKeyDown(Keys.Left))
        {
            player.Direction = Enums.Direction.WalkLeft;
            player.Walk = true;
        }
        else if (keyboard.IsKeyDown(Keys.Right))
        {
            player.Direction = Enums.Direction.WalkRight;
            player.Walk = true;
        }

        if (keyboard.IsKeyUp(Keys.Left) && keyboard.IsKeyUp(Keys.Right))
        {
            player.Walk = false;
        }

        if (keyboard.IsKeyDown(Keys.NumPad0) && !jump_key_pressed
        && !player.Jump && player.Ground
             && player.Position.Y > 0)
        {
            jump_key_pressed = true;
            player.Jump = true;
        }

        if (keyboard.IsKeyUp(Keys.NumPad0) )
        {
            jump_key_pressed = false;
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
