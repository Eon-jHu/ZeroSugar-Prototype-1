using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IOccupier
{
    public static Player Instance;

    public Transform OccupierTransform => transform;
    
    [SerializeField]
    private int Health;

    [SerializeField]
    public int actionPoints { get; private set; }

    [SerializeField]
    private Tile currentTile;

    [SerializeField]
    private Board board;

    //Temporary variables
    Card[] Deck;
    Card[] Played;
    Card[] Discarded;

    private void Awake()
    {
        Tile.OnCenterTileAssigned += PlacePlayer;
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
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

    public void EndTurn()
    {
        TurnBasedSystem.Instance.EndPlayerTurn();
    }

    public void ConsumeActionPoints(int actionCost)
    {
        actionPoints -= actionCost;

        // SHOULD HAVE ALREADY CHECKED AV BEFOREHAND!!!
        if (actionPoints < 0) // This would be cheating
        {
            actionPoints = 0;
        }
    }
    
}


