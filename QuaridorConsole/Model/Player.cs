

using System;

public class Player
{
    private GameState _gameState;

    private PlayGrid _playGrid;

    private Vector2 _startPosition;

    private string _name;

    public int _obstacles = 10;

    public Player(GameState gameState, PlayGrid playGrid, string name, int x, int y, bool isAI)
    {
        _gameState = gameState;

        _playGrid = playGrid;

        _startPosition = new Vector2(x, y, 0, null);

        _name = name;

        _playGrid.ChangePlayerPositionOnGrid(isAI, x, y, x, y);

        X = x;

        Y = y;

        IsAI = isAI;
    }

    public int X { get; private set; }

    public int Y { get; private set; }

    public bool IsAI { get; private set; }

    public string Name { get => _name; }

    public bool IsHaveObstacles { get => _obstacles > 0; }

    public bool MovePlayer(int directionX, int directionY)
    {
        int x = X + (directionX * 2);

        int y = Y + (directionY * 2);

        bool isPlayer = false;

        bool isCanMove = _playGrid.IsCanMove(X, Y, x, y);

        if(isCanMove)
            isPlayer = _playGrid.IsPlayerOnZone(x, y);

        if (isPlayer)
        {
            // Check forward
            bool isCanForward =  _playGrid.IsCanMove(X, Y, x + (directionX * 2), y + (directionY * 2));

            // Check Up\Down
            bool isY = _playGrid.IsCanMove(X, Y, x, y + (directionY * 2));

            // Check Left\Right
            bool isX = _playGrid.IsCanMove(X, Y, x + (directionX * 2), y);

            if (isCanForward)
            {
                x += (directionX * 2);

                y += (directionY * 2);
            }
            else if (isY)
            {
                bool isUp = _playGrid.IsCanMove(x, y, x, y + (directionX * 2));

                y = isUp ? y + (directionX * 2) : y - (directionX * 2);
            }
            else if (isX)
            {
                bool isLeft = _playGrid.IsCanMove(x, y, x + (directionY * 2), y);

                x = isLeft ? x + (directionY * 2) : x - (directionY * 2);
            }
        }
        else if (isCanMove == false) return false;

        _playGrid.ChangePlayerPositionOnGrid(IsAI, X, Y, x, y);

        Console.WriteLine("Position: " + x + "  " + y);

        ChangePosition(x, y);

        return true;
    }

    private void ChangePosition(int x, int y)
    {
        X = x;

        Y = y;

        int winPosition = _startPosition.X == 0 ? _playGrid.PlayZoneHight - 1 : 0;

        if (winPosition == X)
        {
            _gameState.GameFinish(this);
        }
        else
        {
            _gameState.OnNextStep();
        }
    }

    public void ClearPlayer()
    {
        X = _startPosition.X;

        Y = _startPosition.Y;

        _obstacles = 10;

        _playGrid.ChangePlayerPositionOnGrid(IsAI, X, Y, X, Y);
    }

    ~Player()
    {
        
    }
}
