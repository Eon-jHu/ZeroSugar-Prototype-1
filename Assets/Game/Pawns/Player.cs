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

    public bool CheckActionValue(int value)
    {
        return actionPoints >= value;
    }    

    private void Awake()
    {
        Tile.OnCenterTileAssigned += PlacePlayer;
        actionPoints = 3;
    }

    private void PlacePlayer(Tile tile)
    {
        tile.OccupyTile(this, null, true);
    }

    private void OnDestroy()
    {
        Tile.OnCenterTileAssigned -= PlacePlayer;
    }
}


