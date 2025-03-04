using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IOccupier
{
    #region Variables
    public Transform OccupierTransform => transform;


    public enum eEnemyType
    {
        MELEE,
        RANGED,
    }

    [SerializeField]
    public eEnemyType enemyType;

    [SerializeField]
    public Transform player;

    [SerializeField]
    public float attackRange;

    [SerializeField]
    public int attackDamage;

    [SerializeField]
    public LayerMask playerLayer;


    [SerializeField]
    private int Health;

    [SerializeField]
    private Board board;

    [SerializeField]
    private float moveSpeed;

    [SerializeField, Tooltip("How long the enemy can animate moving before teleporting to intended tile/destination")] 
    private float maximumMoveTime = 4;

    private bool isMoving = false;

    [SerializeField] 
    private int health = 3;

    private int maxHealth;

    private HealthBar healthBar;

    #endregion

    #region Initialization
    void Awake()
    {
        //Board.OnBoardReady += EnemySpawn;
        //Board.OnBoardReady?.Invoke(this);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        maxHealth = health;
        EnemySpawn(board);
    }

    #endregion

    #region Update
    void Update()
    {
        
    }

    #endregion

    #region Functions
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
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar ??= GetComponentInChildren<HealthBar>();
        if (healthBar)
        {
            healthBar.UpdateHealthBar((float)health / maxHealth);
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Board boardRef)
    {
        board = boardRef;
    }

    public IEnumerator ProcessTurn(Action completeTurn)
    {
        bool isInAttackRange = false; // calculate if is in attack range.

        isInAttackRange = EnemyRangeCheck();

        if (isInAttackRange)
        {
            Debug.Log("In Range");
            // attack
            Attack();
            // ....
            completeTurn?.Invoke();
        }
        else
        {
            Debug.Log("NOT in Range");
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
            bool canOccupy = targetTile.OccupyTile(this, currentTile);

            if (!canOccupy) yield break;

            //animate enemy movement e.g Animator.SetBool("Walk", true)

            isMoving = true;

            while (moveTimeRemaining > 0)
            {
                moveTimeRemaining -= Time.deltaTime;
                Vector3 directionToTile = (targetDest - transform.position).normalized;

                transform.position += moveSpeed * Time.deltaTime * directionToTile;

                transform.forward = directionToTile;

                yield return null;
            }
            // snap to the center tile at the end of the move sequence, should already be there from the while loop above.
            transform.position = targetTile.transform.position;
            isMoving = false;
            //Animator.SetBool("Walk", false)
        }
        isMoving = false;
    }

    private bool EnemyRangeCheck()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if within attack range
        if (distanceToPlayer <= attackRange)
        {
            // Perform Raycast
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, attackRange, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position, directionToPlayer.normalized * hit.distance, Color.red, 2.0f); // Debug
                    
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    
    }

    private void PlaceEnemy(Tile tile)
    {
        tile.OccupyTile(this, null, true);
    }

    private void DelayDamage()
    {
        Player.Instance.TakeDamage(attackDamage);
    }
    private void Attack()
    {
        //seperated by type for the mean time

        if (enemyType == eEnemyType.MELEE)
        {
            //melee attack
            GetComponent<Animator>().SetBool("TailAttack", true);
            Invoke("DelayDamage", 0.8f);
        }
        if (enemyType == eEnemyType.RANGED)
        {
            //ranged attack
            Projectile.CreateProjectile(transform, board.GetPlayerTile(), "Projectile Enemy");
            AudioPlayer.PlaySound3D(Sound.weapon_throw, transform.position);
            Player.Instance.TakeDamage(attackDamage);
        }
    }

    #endregion
}
