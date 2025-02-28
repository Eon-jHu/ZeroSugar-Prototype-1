using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    #region Variables
    [SerializeField] 
    private GameObject enemyMelee;
    [SerializeField]
    private GameObject enemyRanged;

    [SerializeField]
    private List<Enemy> enemyList;

    [SerializeField]
    private int spawnPhaseCount;

    bool hasSpawned = false;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemyMelee);
        //Instantiate(enemyRanged);
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        Spawn();
    }
    #endregion

    #region Fuctions
    void Spawn()
    {

        if (TurnBasedSystem.Instance.turnPhase - 1 % spawnPhaseCount != 0)
        {
            hasSpawned = false; // Reset when it's not the spawn phase
        }

        if (!hasSpawned && TurnBasedSystem.Instance.turnPhase - 1 % spawnPhaseCount == 0)
        {
            Instantiate(enemyMelee);
            Instantiate(enemyRanged);
            Debug.Log("Spawn new Enemy");

            hasSpawned = true;
        }
    }
    #endregion
}
