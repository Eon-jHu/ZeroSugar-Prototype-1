using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedSystem : MonoBehaviour
{
    public static TurnBasedSystem Instance;

    private enum TurnState { PlayerTurn, EnemyTurn}
    private TurnState currentTurn;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartPlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EndPlayerTurn()
    {
        Debug.Log("Player Turn Ended");
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        Debug.Log("Enemy Turn Started");

        yield return new WaitForSeconds(1f); // Simulate enemy thinking time

        // enemy instance

        /*  Enemy Actions
            - movement
            - if in range, attack
            - spawn next wave if spawn conditions are met
                - spawn conditions depend on wave
                - x amount of enemy movement turn and conditions are met
            - end turn
        */
        Debug.Log("Enemy actions");
        yield return new WaitForSeconds(1f); // Wait before switching back

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        Debug.Log("Player Turn Started");
        //player instance
    }
}
