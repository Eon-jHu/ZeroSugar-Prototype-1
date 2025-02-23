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
        
    }

    private void EnemySpawn(Board board)
    {
        List<Tile> tiles = board.GetTiles();

        //foreach (Tile tileInstance in tiles)
        //{
        //    if (tileInstance.IsBoundaryTile == true)
        //    {

        //    }
        //}
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
