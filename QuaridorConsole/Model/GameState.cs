using System;

public class GameState
{
    public Action WhenStart;

    public Action WhenNextStep;

    public Action WhenFinish;

    public void Start()
    {
        WhenStart?.Invoke();
    }

    public void OnNextStep()
    {
        WhenNextStep?.Invoke();
    }

    public void GameFinish(Player winner)
    {
        WhenFinish?.Invoke();

        Console.WriteLine("Winner: " + winner.Name);
    }
}
