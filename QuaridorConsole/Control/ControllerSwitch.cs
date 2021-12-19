using System;

public enum ControllerInput { Inspector, AI }

public class ControllerSwitch
{
    private GameState _gameState;

    private Controller[] _controllers;

    private int _activeController;

    public ControllerSwitch(GameState gameState, Controller[] controllers)
    {
        _gameState = gameState;

        _controllers = controllers;

        _gameState.WhenNextStep += NextStep;

        Console.Write("-> ");
    }

    public Controller ActualController { get => _controllers[_activeController]; }

    private void NextStep()
    {
        if (_activeController + 1 < _controllers.Length)
        {
            _activeController++;
        }
        else
        {
            _activeController = 0;
        }

        Console.Write(ActualController.ControllerInput == ControllerInput.AI ? "<- " : "-> ");

        ActualController.Activate();
    }

    public bool IsCurrectControllerInput(ControllerInput controllerInput)
    {
        return ActualController.ControllerInput.Equals(controllerInput);
    }
}
