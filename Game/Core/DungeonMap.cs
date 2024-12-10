using System.Collections.ObjectModel;
using RLNET;
using RogueSharp;

namespace Game.Core;

public class DungeonCell : Cell
{
    public bool IsExplored
    {
        get;
        set;
    }
}

public class DungeonMap : Map<DungeonCell>
{
    private readonly FieldOfView<DungeonCell> _fieldOfView;

    public DungeonMap()
    {
        _fieldOfView = new FieldOfView<DungeonCell>(this);
    }

    public bool IsExplored(int x, int y)
    {
        return this[x, y].IsExplored;
    }

    public void UpdatePlayerFieldOfView()
    {
        Player player = Game.Player;
        ComputeFov(player.X, player.Y, player.Awareness, true);
        foreach (Cell cell in GetAllCells())
        {
            if (IsInFov(cell.X, cell.Y))
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
    }
    
    public ReadOnlyCollection<DungeonCell> ComputeFov( int xOrigin, int yOrigin, int radius, bool lightWalls )
    {
        return _fieldOfView.ComputeFov( xOrigin, yOrigin, radius, lightWalls );
    }

    public bool SetActorPosition(Actor actor, int x, int y)
    {
        if (GetCell(x, y).IsWalkable)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            actor.X = x;
            actor.Y = y;
            SetIsWalkable(actor.X, actor.Y, false);

            if (actor is Player)
            {
                UpdatePlayerFieldOfView();
            }

            return true;
        }

        return false;
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        DungeonCell cell = GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }
    
    public void SetCellProperties( int x, int y, bool isTransparent, bool isWalkable, bool isExplored )
    {
        this[x, y].IsTransparent = isTransparent;
        this[x, y].IsWalkable = isWalkable;
        this[x, y].IsExplored = isExplored;
    }
    
    public bool IsInFov( int x, int y )
    {
        return _fieldOfView.IsInFov( x, y );
    }
    public void Draw(RLConsole mapConsole)
    {
        mapConsole.Clear();
        foreach (DungeonCell dungeonCell in GetAllCells())
        {
            SetConsoleSymbolForCell(mapConsole, dungeonCell);
        }
    }

    private void SetConsoleSymbolForCell(RLConsole console, DungeonCell cell)
    {
        if (!cell.IsExplored)
        {
            return;
        }
       

        if (IsInFov(cell.X, cell.Y))
        {
            if (cell.IsWalkable)
            {
                console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
            }
            else
            {
                console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
            }
        }
        else
        {
            if (cell.IsWalkable)
            {
                console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
            }
            else
            {
                console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
            }
        }
    }
}