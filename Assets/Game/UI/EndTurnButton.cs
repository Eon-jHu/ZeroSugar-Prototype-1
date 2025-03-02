using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    private TurnBasedSystem turnSystem;
    
    public void EndTurn()
    {
        turnSystem ??= FindObjectOfType<TurnBasedSystem>();

        if (!turnSystem)
        {
            Debug.LogError("No TurnBasedSystem script found in scene. Cannot end turn!");
        }

        if (turnSystem.CurrentTurn == TurnBasedSystem.TurnState.PlayerTurn)
        {
            turnSystem.EndPlayerTurn();
        }
    }
}
