
public class PlayGrid
{
    private GridView _gridView;

    private AStar _aStar;

    private int _playZone;

    private int[,] _grid;

    private int _obstacleID = -2;

    private int _obstacleZone = -1;

    private int _playerMoveId = 0;

    private int _playerID = 1;

    private int _aiID = 2;

    public PlayGrid(GridView gridView, int playZone)
    {
        _aStar = new AStar();

        _gridView = gridView;

        _playZone = playZone;

        _grid = GenerateGrid(_playZone);
    }

    public int PlayZoneHight { get => _playZone + (_playZone - 1); }

    public int[,] Grid { get => _grid; }

    private int[,] GenerateGrid(int playZone)
    {
        int[,] grid = new int[PlayZoneHight, PlayZoneHight];

        for (int y = 0; y < playZone + (playZone - 1); y++)
        {
            for (int x = 0; x < playZone + (playZone - 1); x++)
            {
                int zone = x % 2 == 0 && y % 2 == 0 ? _playerMoveId : _obstacleZone;

                zone = x % 2 != 0 && y % 2 != 0 ? _obstacleID : zone;

                grid[y, x] = zone;
            }
        }

        return grid;
    }

    public void SetGrid(int[,] newGrid)
    {
        for (int y = 0; y < PlayZoneHight; y++)
        {
            for (int x = 0; x < PlayZoneHight; x++)
            {
                _grid[y, x] = newGrid[y, x];
            }
        }
    }

    public bool SetObstacle(int indexY, int indexX)
    {
        bool isHorizontal = indexX % 2 == 0;

        bool isSuccess = false;

        int rows = _grid.GetUpperBound(0) + 1;    

        int columns = _grid.Length / rows;

        if (_grid[indexX, indexY] == -1)
        {
            bool isCanPass = true;

            if (isHorizontal)
            {
                bool isIndexOut = rows <= indexX + 1 || columns <= indexY + 1;

                if (isIndexOut || _grid[indexX + 1, indexY + 1] == -2 || _grid[indexX + 2, indexY] == -2)
                {
                    return false;
                }
                else
                {
                    _grid[indexX, indexY] = _obstacleID;
                    _grid[indexX + 1, indexY] = _obstacleID;
                    _grid[indexX + 2, indexY] = _obstacleID;

                    isCanPass = IsCanPass();

                    if (isCanPass == false)
                    {
                        _grid[indexX, indexY] = _obstacleZone;
                        _grid[indexX + 1, indexY] = _obstacleZone;
                        _grid[indexX + 2, indexY] = _obstacleZone;

                        return false;
                    }
                }
            }
            else
            {
                bool isIndexOut = rows <= indexX + 1 || columns <= indexY + 1;

                if (isIndexOut || _grid[indexX + 1, indexY + 1] == -2 || _grid[indexX, indexY + 2] == -2)
                {
                    return false;
                }
                else
                {
                    _grid[indexX, indexY] = _obstacleID;
                    _grid[indexX, indexY + 1] = _obstacleID;
                    _grid[indexX, indexY + 2] = _obstacleID;

                    isCanPass = IsCanPass();

                    if (isCanPass == false)
                    {
                        _grid[indexX, indexY] = _obstacleZone;
                        _grid[indexX + 1, indexY] = _obstacleZone;
                        _grid[indexX + 2, indexY] = _obstacleZone;

                        return false;
                    }
                }
            }

            isSuccess = true;
        }

        RefreshGrid();

        return isSuccess;
    }

    public int GetDistanceToTarget(Vector2 from, Vector2 target)
    {
        return _aStar.GetDistanceToTarget(from, target, _grid);
    }

    public void FindWay()
    {
        Vector2 startVector = new Vector2(0, 4, 1, null);

        Vector2 targetVector = new Vector2(PlayZoneHight - 1, (PlayZoneHight / 2) - 1, 1, null);

        _aStar.FindWay(startVector, targetVector, _grid);

        RefreshGrid();
    }

    public Vector2 FindPlayerPosition()
    {
        int rows = _grid.GetUpperBound(0) + 1;    // количество строк
        int columns = _grid.Length / rows;        // количество столбцов

        for (int y = 0; y < columns; y++)
        {
            for (int x = 0; x < rows; x++)
            {
               // Debug.Log(x + "  " + y + "  " + _grid[y, x]);

                if (_grid[y, x] == _playerID)
                {
                    return new Vector2(x, y, 0, null);
                }
            }
        }

        return new Vector2(0, 8, 0, null);
    }

    public bool IsCanPass()
    {
        Vector2 startVector = new Vector2(0, 4, 1, null);

        Vector2 targetVector = new Vector2(PlayZoneHight - 1, PlayZoneHight / 2, 1, null);

        return _aStar.FindWay(startVector, targetVector, _grid);
    }

    public bool ChangePlayerPositionOnGrid(bool isAi, int oldX, int oldY, int directionX, int directionY)
    {
        _grid[oldX, oldY] = _playerMoveId;

        _grid[directionX, directionY] = isAi ? _aiID : _playerID;

        RefreshGrid();

        return true;
    }

    public void RefreshGrid()
    {
        _gridView?.RefreshGridView?.Invoke(_grid);
    }

    public bool IsPlayerOnZone(int x, int y)
    {
        return _grid[x, y] == _playerID || _grid[x, y] == _aiID;
    }

    public bool IsCanMove(int xFrom, int yFrom, int xTo, int yTo)
    {
        if (yFrom < 0 || xFrom < 0) return false;

        if (IsInPlayZone(xTo, yTo, PlayZoneHight) == false) return false;


        if (xFrom != xTo)
        {
            int direction = (xTo - xFrom) / 2;

            direction = direction > 0 ? 1 : -1;

            return _grid[xTo - direction, yTo] == _obstacleZone;
        }
        else if (yFrom != yTo)
        {
            int direction = (yTo - yFrom) / 2;

            direction = direction > 0 ? 1 : -1;

            return _grid[xTo, yTo - direction] == _obstacleZone;
        }

        return false;
    }

    public bool IsInPlayZone(int x, int y, int maxSize)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }
        else if (x > maxSize || y > maxSize)
        {
            return false;
        }

        return true;
    }

    public void ClearGrid()
    {
        _grid = GenerateGrid(_playZone);
    }
}
