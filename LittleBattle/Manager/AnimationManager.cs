using LittleBattle.Classes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class AnimationManager
{
    private readonly Dictionary<object, Animation> _anims = new Dictionary<object, Animation>();
    private object _lastKey;

    public void AddAnimation(object key, Animation animation)
    {
        _anims.Add(key, animation);
        _lastKey = key;
    }

    public void Update(object key, bool fastAnimation, int specificFrame = -1)
    {
        if (_anims.TryGetValue(key, out Animation value))
        {
            value.Start();
            _anims[key].Update(fastAnimation, specificFrame);
            _lastKey = key;
        }
    }

    public Animation GetAnimation(object key)
    {
        return _anims[key];
    }

    public void Draw(Vector2 position, float layerdepth = 0.9f, float alpha = 1f)
    {
        _anims[_lastKey].Draw(position, layerdepth, alpha);
    }

    public Rectangle instantRect()
    {
        return _anims[_lastKey].instantRect();
    }

    public void SetLoop(bool loop)
    {
        _anims[_lastKey].SetLoop(loop);
    }
}

