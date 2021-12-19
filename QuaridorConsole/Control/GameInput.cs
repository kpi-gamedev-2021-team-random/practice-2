using System;
using System.Collections.Generic;
using System.Text;

namespace QuaridorConsole.Control
{
    public class GameInput
    {
        private ControllerSwitch _controllerSwitch;

        private GameState _gameState;

        private PlayGrid _playGrid;

        private int _obstacleX;

        private int _obstacleY;

        public GameInput(ControllerSwitch controllerSwitch, GameState gameState, PlayGrid playGrid)
        {
            _controllerSwitch = controllerSwitch;

            _gameState = gameState;

            _playGrid = playGrid;

            Update();
        }

        private void Update()
        {
            while (true)
            {
                string command = Console.ReadLine();

                switch (command)
                {
                    case "move up":
                        PlayerMoveUp();
                        break;
                    case "move down":
                        PlayerMoveDown();
                        break;
                    case "move left":
                        PlayerMoveLeft();
                        break;
                    case "move right":
                        PlayerMoveRight();
                        break;
                    case "wall":
                        Console.WriteLine("wall position: ");

                        Console.Write("Set X: ");
                        _obstacleX = Int32.Parse(Console.ReadLine());

                        Console.Write("Set Y: ");
                        _obstacleY = Int32.Parse(Console.ReadLine());

                        SetObstacle();
                        break;
                    default:
                        break;
                }
            }
        }

        public void Restart()
        {
            _gameState.GameFinish(null);
        }

        public void FindWay()
        {
            _playGrid.FindWay();
        }

        public void SetObstacle()
        {
            if (_controllerSwitch.IsCurrectControllerInput(ControllerInput.Inspector))
            {
                bool isSuccess = _controllerSwitch.ActualController.PlayerSetObstacle(_obstacleX, _obstacleY);

                if (isSuccess == false)
                    Console.WriteLine("Can not set the obstacle in the position " + _obstacleX +"  "+ _obstacleY);
            }
        }

        public void PlayerMoveUp()
        {
            if (_controllerSwitch.IsCurrectControllerInput(ControllerInput.Inspector))
                _controllerSwitch.ActualController.PlayerMoveUp();
        }

        public void PlayerMoveDown()
        {
            if (_controllerSwitch.IsCurrectControllerInput(ControllerInput.Inspector))
                _controllerSwitch.ActualController.PlayerMoveDown();
        }

        public void PlayerMoveRight()
        {
            if (_controllerSwitch.IsCurrectControllerInput(ControllerInput.Inspector))
                _controllerSwitch.ActualController.PlayerMoveRight();
        }

        public void PlayerMoveLeft()
        {
            if (_controllerSwitch.IsCurrectControllerInput(ControllerInput.Inspector))
                _controllerSwitch.ActualController.PlayerMoveLeft();
        }
    }
}
