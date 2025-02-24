using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IOccupier
{
    public Transform OccupierTransform => transform;

    public enum eEnemyType
    {
        MELEE,
        RANGED,
    }

    [SerializeField]
    private int Health;

    [SerializeField]
    private Board board;

    [SerializeField]
    private float moveSpeed;

    [SerializeField, Tooltip("How long the enemy can animate moving before teleporting to intended tile/destination")] 
    private float maximumMoveTime = 4;

    private bool isMoving = false;


    // Start is called before the first frame update
    void Awake()
    {
        Board.OnBoardReady += EnemySpawn;
    }

    private void EnemySpawn(Board board)
    {
        List<Tile> tiles = board.GetTiles();
        List<Tile> spawnableTiles = new();

        foreach (Tile tileInstance in tiles)
        {
            if (tileInstance.IsBoundaryTile() == true && tileInstance.IsOccupied() == false)
            {
                spawnableTiles.Add(tileInstance);
                
                

            }
        }
        Tile randomTile = spawnableTiles[Random.Range(0, spawnableTiles.Count)];
        PlaceEnemy(randomTile);

    }

    public IEnumerator ProcessTurn(Action completeTurn)
    {
        bool isInAttackRange = false; // calculate if is in attack range.

        if (isInAttackRange)
        {
            // attack
            // ....
            completeTurn?.Invoke();
        }
        else
        {
            if (!isMoving)
            {
                StartCoroutine(MoveTowardsPlayer());

                yield return new WaitUntil(() => !isMoving);
                
                completeTurn?.Invoke();
            }
        }
    }

    private IEnumerator MoveTowardsPlayer()
    {
        Tile currentTile = Board.Instance.GetOccupierTile(this);

        if (currentTile)
        {
            Tile targetTile = Board.Instance.GetNextTileOnPathToPlayer(currentTile);

            if (!targetTile) yield break;
            
            Vector3 targetDest = targetTile.transform.position;

            float distance = Vector3.Distance(transform.position, targetDest);

            // do not allow a unit to take longer than 5 seconds to move.
            float moveTimeRemaining = Mathf.Min(distance / moveSpeed, maximumMoveTime);
            targetTile.OccupyTile(this, currentTile);
            
            //animate enemy movement e.g Animator.SetBool("Walk", true)

            isMoving = true;

            while (moveTimeRemaining > 0)
            {
                moveTimeRemaining -= Time.deltaTime;
                Vector3 directionToTile = (targetDest - transform.position).normalized;
                
                transform.Translate(moveSpeed * Time.deltaTime * directionToTile);
                // need to test this, I think this is how to face the target tile.
                transform.LookAt(new Vector3(targetDest.x, transform.position.y, targetDest.z));

                
                yield return null;
            }
            transform.position = targetTile.transform.position;
            isMoving = false;
            //Animator.SetBool("Walk", false)
        }
        isMoving = false;
    }

    private void PlaceEnemy(Tile tile)
    {
        tile.OccupyTile(this, null, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
