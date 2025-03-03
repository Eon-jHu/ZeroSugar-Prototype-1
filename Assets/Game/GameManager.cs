using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Variables
    public Timer turnTimer;
    public float turnTime = 15f;
    public Card cardBeingPlayed = null;

    // References
    private Board board;
    public Player player;

    [SerializeField] private GameObject weaponProjectile;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelCard();
        }
    }

    // Play the card (subscribed to tile click event)
    public void PlayCard(Card card)
    {
        // Check if they're already playing a card
        if (cardBeingPlayed)
        {
            CancelCard();
        }
        cardBeingPlayed = card;

        // 0. Check if they have enough Action Value to play this card
        if (player.actionPoints < card.cardData.actionCost)
        {
            Debug.Log("Not enough action points to play this card.");
            CancelCard();
            return;
        }

        // 1. Visually display the card's range on the board based on mouse position
        board.ShowRange(board.GetPlayerTile(), card.cardData.maxRange);

        // 2. Subscribe the card's effect to the tile click event
        Tile.OnTileClicked += ResolveCard;

        // 3. We just need to wait for the player to click a tile, or press escape to cancel
        
    }

    private void CancelCard()
    {
        if (cardBeingPlayed)
        {
            cardBeingPlayed.ResetCardInHand();
            cardBeingPlayed = null;
            board.DisableShowRange();
            Tile.OnTileClicked -= ResolveCard;
        }
    }

    // Triggers when a tile is clicked
    private void ResolveCard(Tile tile)
    {
        // 4. Consume action points
        player.ConsumeActionPoints(cardBeingPlayed.cardData.actionCost);

        // 5. Assign damage to any enemies on the tile
        HandleAttack(tile);

        // 6. Discard the card
        cardBeingPlayed.MoveToDiscardPile();

        // 7. End the player's turn if their action points are now 0
        //if (player.actionPoints <= 0)
        //{
        //    player.EndTurn();
        //}

        // 8. Reset the board
        board.DisableShowRange();

        // 9. Unsubscribe from the tile click event
        Tile.OnTileClicked -= ResolveCard;

        // 10. Clean up
        cardBeingPlayed = null;
    }

    private void HandleAttack(Tile tile)
    {
        if (tile.Occupier != null)
        {
            player.AnimateAttack(tile);

            if (tile.Occupier.OccupierTransform.TryGetComponent(out Enemy enemy))
            {
                bool isInOptimalRange =
                    board.TileIsInOptimalRange(board.GetPlayerTile(), tile, cardBeingPlayed.cardData.maxRange);
                
                // if player is outside of optimal range for the selected card, half the damage & floor it.
                int damage = Mathf.Max(1, isInOptimalRange ? cardBeingPlayed.cardData.damage : 
                    Mathf.FloorToInt((float)cardBeingPlayed.cardData.damage / 2));

                float animationReleaseTime = 0.5f;
                this.Wait(animationReleaseTime, () =>
                {
                    Vector3 spawnPos = player.transform.position + Vector3.up * 2;
                    Projectile projectile = Instantiate(weaponProjectile, spawnPos, player.transform.rotation)
                        .GetComponent<Projectile>();
                    projectile.SetProjectile(tile.transform.position + Vector3.up, 40);

                    enemy.TakeDamage(damage);
                });
            }
        }
    }
}
