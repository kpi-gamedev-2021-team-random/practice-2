using Assets.Source.Control;
using QuaridorConsole.Control;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuaridorConsole
{
    public class Main
    {
        private const int PlayZoneSize = 9;

        private const int Height = PlayZoneSize + (PlayZoneSize - 2);

        private int _obstacleIndexX;

        private int _obstacleIndexY;

        private ControllerSwitch _controllerSwitch;

        private GameState _gameState;

        private GridView _gridView;

        private PlayGrid _playGrid;

        private GameInput _gameInput;

        public Main()
        {
            _gameState = new GameState();

            _gridView = new GridView();

            _playGrid = new PlayGrid(_gridView, PlayZoneSize);

            Player mainPlayer = new Player(_gameState, _playGrid, "Player", 0, Height / 2, false);

            Player secondPlayer = new Player(_gameState, _playGrid, "AI", Height, Height / 2, true);

            Controller player = new PlayerController(mainPlayer, _playGrid, _gameState);

            Controller ai = new AIController(secondPlayer, _playGrid, _gameState);

            _controllerSwitch = new ControllerSwitch(_gameState, new Controller[] { player, ai });

            SubscribeOnActions();

            _playGrid.RefreshGrid();

            _gameInput = new GameInput(_controllerSwitch, _gameState, _playGrid);
        }

        private void SubscribeOnActions()
        {
            _gameState.WhenFinish += _playGrid.ClearGrid;

            _gameState.WhenFinish += _playGrid.RefreshGrid;
        }
    }
}
