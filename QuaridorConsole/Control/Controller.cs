
public class Controller
{
    private GameState _gameState;

    private PlayGrid _playGrid;

    protected Player _player;

    public Controller(Player player, PlayGrid playGrid, GameState gameState)
    {
        _player = player;

        _gameState = gameState;

        _playGrid = playGrid;

        _gameState.WhenFinish += _player.ClearPlayer;
    }

    public ControllerInput ControllerInput { get; protected set; }

    private bool IsPlayerTurn()
    {
        return true;
    }

    public virtual void Activate()
    {

    }

    public bool PlayerMoveUp()
    {
        if (IsPlayerTurn())
        {
            return _player.MovePlayer(0, 1);
        }

        return false;
    }

    public bool PlayerMoveDown()
    {
        if (IsPlayerTurn())
        {
            return _player.MovePlayer(0, -1);
        }

        return false;
    }

    public bool PlayerMoveRight()
    {
        if (IsPlayerTurn())
        {
            return _player.MovePlayer(-1, 0);
        }

        return false;
    }

    public bool PlayerMoveLeft()
    {
        if (IsPlayerTurn())
        {
            return _player.MovePlayer(1, 0);
        }

        return false;
    }

    public bool PlayerSetObstacle(int x, int y)
    {
        bool isSuccess = false;

        if (IsPlayerTurn())
        {
            if (_player.IsHaveObstacles)
            {
                isSuccess = _playGrid.SetObstacle(y, x);

                if (isSuccess)
                {
                    _player._obstacles--;

                    _gameState.OnNextStep();
                }
            }
        }

        return isSuccess;
    }

    public void NextStep()
    {
        _gameState.OnNextStep();
    }
}
