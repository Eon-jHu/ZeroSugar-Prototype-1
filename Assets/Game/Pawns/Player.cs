using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IOccupier
{
    public Transform OccupierTransform => transform;
    
    [SerializeField]
    private int Health;

    [SerializeField]
    private int actionPoints;

    [SerializeField]
    private Tile currentTile;

    [SerializeField]
    private Board board;

    //Temporary variables
    Card[] Deck;
    Card[] Played;
    Card[] Discarded;

    // Start is called before the first frame update
    void Awake()
    {
        Tile.OnCenterTileAssigned += PlacePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlacePlayer(Tile tile)
    {
        tile.OccupyTile(this, null);
    }

    private void OnDestroy()
    {
        Tile.OnCenterTileAssigned -= PlacePlayer;
    }

}
