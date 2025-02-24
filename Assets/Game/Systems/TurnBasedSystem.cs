using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        /*  Enemy Actions
        - movement
        - if in range, attack
        - cycle to next enemy in enemy list
        
        - spawn next wave if spawn conditions are met
            - spawn conditions depend on wave
            - x amount of enemy movement turn and conditions are met
        - end turn
    */

        List<Enemy> enemies = FindObjectsOfType<Enemy>().ToList();

        foreach (var enemy in enemies)
        {
            bool turnComplete = false;
            
            /* ProcessTurn coroutine started on the enemy, once the enemy unit has completed their turn, the coroutine executes
             the passed in lambda function to make turnComplete true which allows moving passed the WaitUntil line below.
             */
            enemy.StartCoroutine(enemy.ProcessTurn(() => turnComplete = true));

            yield return new WaitUntil(() => turnComplete);
            
            yield return new WaitForSeconds(1f); // Pause before moving onto the next enemy unit.
        }
        
        
        Debug.Log("Enemy actions");
        yield return new WaitForSeconds(1f); // Wait before switching back

        StartPlayerTurn();
    }
    

    private void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        Debug.Log("Player Turn Started");
        Player.Instance.EndTurn();
    }
}
