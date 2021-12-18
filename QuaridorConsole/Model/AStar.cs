using System.Collections.Generic;

public class AStar
{
    private int[,] _grid;

    private int[,] _weightGrid;

    private Queue<Vector2> _point;

    private Vector2 _lastVector;

    private Vector2 _targetVector;

    private int _rows;

    private int _columns;

    public void Init(int[,] grid)
    {
        _grid = grid;

        _rows = grid.GetUpperBound(0) + 1;

        _columns = grid.Length / _rows;

        _weightGrid = new int[_columns, _rows];

        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                _weightGrid[i, j] = _grid[i, j];
            }
        }

        _point = new Queue<Vector2>();
    }

    public bool FindWay(Vector2 from, Vector2 target, int[,] grid)
    {
        Init(grid);

        _targetVector = target;

        _weightGrid[from.Y, from.X] = 1;

        _point.Enqueue(new Vector2(from.Y, from.X, 1, null));

        bool isFind = SearchTargetVector();

        if (isFind)
        {
            Way();
        }

        return isFind;
    }

    public int GetDistanceToTarget(Vector2 from, Vector2 target, int[,] grid)
    {
        Init(grid);

        _targetVector = target;

        _weightGrid[from.Y, from.X] = 1;

        _point.Enqueue(new Vector2(from.Y, from.X, 1, null));

        bool isFind = SearchTargetVector();

        if (isFind)
        {
            return Way();
        }

        return -1;
    }

    private bool SearchTargetVector()
    {
        for (int i = 2; i < _weightGrid.Length; i++)
        {
            Vector2 vector;

            if (_point.Count > 0)
                vector = _point.Dequeue();
            else 
                break;


            if (_targetVector.X == vector.X && _targetVector.Y == vector.Y)
            {
                _lastVector = vector;

                return true;
            }

            Star(vector.X, vector.Y, vector.Index, vector);
        }

        return false;
    }

    private void Star(int x, int y, int count, Vector2 vector)
    {
        Vector2 up = new Vector2(x, y + 1, count  + 1, vector);
        Vector2 down = new Vector2(x, y - 1, count + 1, vector);

        Vector2 left = new Vector2(x - 1, y, count + 1, vector);
        Vector2 right = new Vector2(x + 1, y, count + 1, vector);

        AddVectorToQueue(up);
        AddVectorToQueue(down);

        AddVectorToQueue(left);
        AddVectorToQueue(right);
    }

    private void AddVectorToQueue(Vector2 vector)
    {
        if ((vector.Y >= 0 && vector.X >= 0 && vector.Y < _rows && vector.X < _rows) == false)
            return;


        if (_weightGrid[vector.Y, vector.X] == 0 || _weightGrid[vector.Y, vector.X] == -1 || _weightGrid[vector.Y, vector.X] == 1)
        {
            _weightGrid[vector.Y, vector.X] = vector.Index;

            _point.Enqueue(vector);
        }
    }

    private int Way()
    {
        int steps = 0;

        while (_lastVector != null)
        {
            //_grid[_lastVector.Y, _lastVector.X] = -4;

            _lastVector = _lastVector.PreviousVector;

            steps++;
        }

        return steps;
    }
}

public class Vector2
{
    public Vector2 PreviousVector;

    public int X;

    public int Y;

    public int Index;

    public Vector2(int x, int y, int index, Vector2 vector)
    {
        X = x;

        Y = y;

        Index = index;

        PreviousVector = vector;
    }
}
