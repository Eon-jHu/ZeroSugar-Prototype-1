using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour, IOccupier
{
    #region Variables
    public static Player Instance;

    public Transform OccupierTransform => transform;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI actionPointsText;

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

    #endregion

    #region Initialization
    private void Awake()
    {
        Tile.OnCenterTileAssigned += PlacePlayer;
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        actionPoints = 3;
    }
    #endregion

    private void Update()
    {
        healthText.text = Health.ToString();
        actionPointsText.text = actionPoints.ToString();
    }

    #region Functions
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
    #endregion

}


