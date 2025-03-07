using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private int cardSelectionPhase;

    bool hasSpawned = false;
    bool sceneSelect = true;
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
        CardSelect();
    }
    #endregion

    #region Fuctions
    private void Spawn()
    {

        if ((TurnBasedSystem.Instance.turnPhase - 1) % spawnPhaseCount != 0)
        {
            hasSpawned = false; // Reset when it's not the spawn phase
        }

        if (!hasSpawned && (TurnBasedSystem.Instance.turnPhase - 1) % spawnPhaseCount == 0)
        {
            Instantiate(enemyMelee);
            Instantiate(enemyRanged);
            Instantiate(enemyRanged);
            Debug.Log("Spawn new Enemy");

            hasSpawned = true;
        }
    }

    private void CardSelect()
    {
        if (TurnBasedSystem.Instance.turnPhase == 0)
            return; // Prevent spawning on turn 0

        if ((TurnBasedSystem.Instance.turnPhase - 1) % cardSelectionPhase != 0)
        {
            sceneSelect = false; 
        }

        if (!sceneSelect && (TurnBasedSystem.Instance.turnPhase - 1) % cardSelectionPhase == 0)
        {


            AsyncOperation op = SceneManager.LoadSceneAsync("Card Select", LoadSceneMode.Additive);
            op.completed += SelectionSceneReady;
            sceneSelect = true;
            if(TurnBasedSystem.Instance.getStartingTime() > 6f)
                TurnBasedSystem.Instance.setStartingTime(TurnBasedSystem.Instance.getStartingTime() - 2f);
        }
    }

    private void SelectionSceneReady(AsyncOperation op)
    {
        CardSelector.Instance.SelectCard();
    }
    #endregion
}
