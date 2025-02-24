using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Variables
    public Timer turnTimer;
    public float turnTime = 15f;
    public bool isPlayerTurn = true;

    // References
    private Board board;
    private Player player;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        player = FindObjectOfType<Player>();
    }

    // Play the card
    public void PlayCard(Card card)
    {
        // Check if it's their turn
        if (!isPlayerTurn) return;

        // 0. Check if they have enough Action Value to play this card
        if (!player.CheckActionValue(card.cardData.actionCost)) return;

        // 1. Visually display the card's range on the board based on mouse position
        board.ShowRange(board.GetPlayerTile(), card.cardData.maxRange);

        // 2. Check if they want to cancel
        // 3. Get the tile the player clicked on
        // 4. Assign damage to any enemies on the tile

        // 5. Discard the card
        card.MoveToDiscardPile();

        // 6. End the player's turn if their action points are now 0
    }
}
