using System.Collections.Generic;
using LittleBattle.Classes;

public class BackgroundManager
{
    private readonly List<Layer> _layers;

    public BackgroundManager()
    {
        _layers = new List<Layer>();
    }

    public void AddLayer(Layer layer)
    {
        _layers.Add(layer);
    }

    public void Update(float movement)
    {
        foreach (var layer in _layers)
        {
            layer.Update(movement);
        }
    }

    public void Draw()
    {
        foreach (var layer in _layers)
        {
            layer.Draw();
        }
    }

    public Layer GetLayer3()
    {
        return _layers[3];
    }
}