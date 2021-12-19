

public class PlayerController : Controller
{
    public PlayerController(Player player, PlayGrid playGrid, GameState gameState) : base(player, playGrid, gameState)
    {
        ControllerInput = ControllerInput.Inspector;
    }
}