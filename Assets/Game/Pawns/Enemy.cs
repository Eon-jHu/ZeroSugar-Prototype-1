using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
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

    private void PlaceEnemy(Tile tile)
    {
        tile.OccupyTile(this, null, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
