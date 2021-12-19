using System;
using System.Collections.Generic;

namespace Assets.Source.Control
{
    public class AIController : Controller
    {
        private PlayGrid _playGrid;

        private PlayGrid _newGridForAI;

        private int[,] _moveDirection = new[,] { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } };

        public AIController(Player player, PlayGrid playGrid, GameState gameState) : base(player, playGrid, gameState)
        {
            _playGrid = playGrid;

            ControllerInput = ControllerInput.AI;
        }

        public override void Activate()
        {
            bool isSucces = MadeMiniMaxDecision();

            if (isSucces == false)
            {
                NextStep();
            }
        }

        private bool MadeMiniMaxDecision()
        {
            _newGridForAI = new PlayGrid(null, 9);

            _playGrid.SetGrid(_playGrid.Grid);

            int[] movementScore = new int[] { -1000, -1000, -1000, -1000 };

            int[] obstaclesScore = new int[] { -1000, -1000, -1000, -1000, -1000 };
            int [,] obstacleNum = new int[5,2];

            // CHECK MOVEMENT //
            for (int i = 0; i < movementScore.Length; i++)
            {
                PlayGrid newPlayGrid = new PlayGrid(null, 9);

                newPlayGrid.SetGrid(_playGrid.Grid);

                MovementInfo movementInfo = MovePlayer(ref newPlayGrid, _moveDirection[i, 0], _moveDirection[i, 1], _player.X, _player.Y);

                if (movementInfo.IsSuccesPass)
                {
                    movementScore[i] = MiniMax(0, 3, true, _newGridForAI, _player.X, _player.Y, _player._obstacles, true);
                }
            }

            // CHECK OBSTACLES //
            if (_player._obstacles > 0)
            {
                Random rnd = new Random();

                for (int i = 0; i < obstaclesScore.Length; i++)
                {
                    int x = rnd.Next(0, 16);

                    int y = rnd.Next(0, 16);

                    PlayGrid newPlayGrid = new PlayGrid(null, 9);
                    
                    newPlayGrid.SetGrid(_playGrid.Grid);

                    bool isObstacleSet = newPlayGrid.SetObstacle(y, x);
                    obstacleNum[i, 0] = x;
                    obstacleNum[i, 1] = y;

                    if (isObstacleSet)
                    {
                        obstaclesScore[i] = MiniMax(0, 4, true, _newGridForAI, _player.X, _player.Y, _player._obstacles, false);
                    }
                }
            }


            // CHECK ACTION //
            int maxMovement = -1000;
            int movementIndex = -1;
            
            int maxObstacle = -1000;
            int obstacleIndex = -1;

            for (int i = 0; i < movementScore.Length; i++)
            {
                if (movementScore[i] > maxMovement)
                {
                    maxMovement = movementScore[i];
                    movementIndex = i;
                }
            }

            for (int i = 0; i < obstaclesScore.Length; i++)
            {
                if (obstaclesScore[i] > maxObstacle)
                {
                    maxObstacle = obstaclesScore[i];
                    obstacleIndex = i;
                }
            }




            // ACTION //
            bool isSuccesMove = false;

            if (maxMovement >= maxObstacle || _player._obstacles <= 0) //Move
            {
                switch (movementIndex)
                {
                    case 0:
                        Console.WriteLine("AI move right");
                        isSuccesMove = PlayerMoveRight();
                        break;
                    case 1:
                        Console.WriteLine("AI move down");
                        isSuccesMove = PlayerMoveDown();
                        break;
                    case 2:
                        Console.WriteLine("AI move left");
                        isSuccesMove = PlayerMoveLeft();
                        break;
                    case 3:
                        Console.WriteLine("AI move up");
                        isSuccesMove = PlayerMoveUp();
                        break;
                    default:
                        break;
                }
            }
            else // Set obstacle
            {
                if (_player._obstacles > 0)
                {
                    int x = obstacleNum[obstacleIndex, 0];
                    int y = obstacleNum[obstacleIndex, 1];

                    isSuccesMove = true;

                    Console.WriteLine("Set Obstacle  " + x + "  " + y + "   obs: " + _player._obstacles);

                    _playGrid.SetObstacle(y, x);

                    _player._obstacles--;

                    NextStep();
                }
            }

            return isSuccesMove;
        }

        private int MiniMax(int depth, int maxDepth, bool isMax, PlayGrid playGrid, int aiXPosition, int aiYPosition, int obstaclesCount, bool isRender)
        {
            if (depth == maxDepth)
            {
                int Value = CalculateScoreByPath(playGrid, aiXPosition, aiYPosition, isRender);

                return Value;
            }

            if (isMax)
            {
                List<int> movementScore = new List<int>();

                List<int> obstaclesScore = new List<int>();

                movementScore = FillMovementScores(depth, maxDepth, true, aiXPosition, aiYPosition, obstaclesCount, isRender);

                if (obstaclesCount > 0)
                {
                    obstaclesScore = FillObstacleScore(depth, maxDepth, true, aiXPosition, aiYPosition, obstaclesCount, isRender);
                }

                int max = -1000;

                for (int i = 0; i < movementScore.Count; i++)
                {
                    if (movementScore[i] > max)
                        max = movementScore[i];
                }

                for (int i = 0; i < obstaclesScore.Count; i++)
                {
                    if (obstaclesScore[i] > max)
                        max = obstaclesScore[i];
                }

                return max;
            }
            else 
            {
                List<int> movementScore = new List<int>();

                List<int> obstaclesScore = new List<int>();

                movementScore = FillMovementScores(depth, maxDepth, false, aiXPosition, aiYPosition, obstaclesCount, isRender);

                if (_player.IsHaveObstacles)
                {
                    obstaclesScore = FillObstacleScore(depth, maxDepth, false, aiXPosition, aiYPosition, obstaclesCount, isRender);
                }

                int min = 1000;

                for (int i = 0; i < movementScore.Count; i++)
                {
                    if (movementScore[i] < min)
                        min = movementScore[i];
                }

                for (int i = 0; i < obstaclesScore.Count; i++)
                {
                    if (obstaclesScore[i] < min)
                        min = obstaclesScore[i];
                }

                return min;
            }

            return 0;
        }

        private List<int> FillMovementScores(int depth, int maxDepth, bool isMax, int aiXPosition, int aiYPosition, int obstaclesCount, bool isRender)
        {
            List<int> movementScores = new List<int>();

            // movement
            for (int i = 0; i < 4; i++)
            {
                PlayGrid newPlayGrid = new PlayGrid(null, 9);
                _playGrid.SetGrid(_playGrid.Grid);

                MovementInfo movementInfo = MovePlayer(ref newPlayGrid, _moveDirection[i, 0], _moveDirection[i, 1], aiXPosition, aiYPosition);

                if (movementInfo.IsSuccesPass)
                {
                    int value = MiniMax(depth + 1, maxDepth, !isMax, newPlayGrid, movementInfo.PositionX, movementInfo.PositionY, obstaclesCount, isRender);

                    movementScores.Add(value);
                }
            }

            return movementScores;
        }

        private List<int> FillObstacleScore(int depth, int maxDepth, bool isMax, int aiXPosition, int aiYPosition, int obstaclesCount, bool isRender)
        {
            List<int> obstaclesScore = new List<int>();

            Random rnd = new Random();

            for (int i = 0; i < 2; i++)
            {
                int x = rnd.Next(0, 16);

                int y = rnd.Next(0, 16);

                PlayGrid newPlayGrid = new PlayGrid(null, 9);

                bool isObstacleSet = newPlayGrid.SetObstacle(y, x);

                obstaclesCount--;

                if (isObstacleSet)
                {
                    int value = MiniMax(depth + 1, maxDepth, !isMax, newPlayGrid, aiXPosition, aiYPosition, obstaclesCount, isRender);

                    obstaclesScore.Add(value);
                }
            }

            return obstaclesScore;
        }

        private int CalculateScoreByPath(PlayGrid playeGrid, int aiXPosition, int aiYPosition, bool isRender)
        {
            Vector2 player = playeGrid.FindPlayerPosition();
            Vector2 ai = new Vector2(aiXPosition, aiYPosition, 0, null);

            Vector2 playerTarget = new Vector2(16, 4, 0, null);
            Vector2 aiTarget = new Vector2(0, 4, 0, null);

            int playerDistance = playeGrid.GetDistanceToTarget(player, playerTarget);
            int aiDistance = playeGrid.GetDistanceToTarget(ai, aiTarget);

            //Debug.Log("From: " + player.X + " " + player.Y + "   To:  " + playerTarget.X + "  " + playerTarget.Y);
            //Debug.Log("From: " + ai.X + " " + ai.Y + "   To:  " + aiTarget.X + "  " + aiTarget.Y);
            //Debug.Log(playerDistance + "  "  + aiDistance);
            //Debug.Log("-------------");


            return playerDistance - aiDistance;
        }

        public MovementInfo MovePlayer(ref PlayGrid playGridAI, int directionX, int directionY, int playerX, int playerY)
        {
            int x = playerX + (directionX * 2);

            int y = playerY + (directionY * 2);

            bool isPlayer = false;

            bool isCanMove = playGridAI.IsCanMove(playerX, playerY, x, y);

            if (isCanMove)
                isPlayer = playGridAI.IsPlayerOnZone(x, y);

            if (isPlayer)
            {
                // Check forward
                bool isCanForward = playGridAI.IsCanMove(playerX, playerY, x + (directionX * 2), y + (directionY * 2));

                // Check Up\Down
                bool isY = playGridAI.IsCanMove(playerX, playerY, x, y + (directionY * 2));

                // Check Left\Right
                bool isX = playGridAI.IsCanMove(playerX, playerY, x + (directionX * 2), y);

                if (isCanForward)
                {
                    x += (directionX * 2);

                    y += (directionY * 2);
                }
                else if (isY)
                {
                    bool isUp = playGridAI.IsCanMove(x, y, x, y + (directionX * 2));

                    y = isUp ? y + (directionX * 2) : y - (directionX * 2);
                }
                else if (isX)
                {
                    bool isLeft = playGridAI.IsCanMove(x, y, x + (directionY * 2), y);

                    x = isLeft ? x + (directionY * 2) : x - (directionY * 2);
                }
            }
            else if (isCanMove == false) return new MovementInfo(false, playerX, playerY);

            playGridAI.ChangePlayerPositionOnGrid(true, playerX, playerY, x, y);

            return new MovementInfo(true, x, y);
        }
    }

    public struct MovementInfo
    {
        public bool IsSuccesPass;

        public int PositionX;

        public int PositionY;

        public MovementInfo(bool isSuccesPass, int x, int y)
        {
            IsSuccesPass = isSuccesPass;

            PositionX = x;

            PositionY = y;
        }
    }
}