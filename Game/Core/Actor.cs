using Game.Interfaces;
using RLNET;
using RogueSharp;

namespace Game.Core;

public class Actor : IActor, IDrawable
{
    public string Name { get; set; }
    public int Awareness { get; set; }
    public RLColor Color { get; set; }
    public char Symbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Draw(RLConsole console, DungeonMap map)
    {
        if (!map.GetCell(X, Y).IsExplored)
        {
            return;
        }

        if (map.IsInFov(X, Y))
        {
            console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
        }
        else
        {
            console.Set(X, Y, Color, Colors.FloorBackground, Symbol);
        }
    }
}