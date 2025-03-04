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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || TurnBasedSystem.Instance.CurrentTurn == TurnBasedSystem.TurnState.EnemyTurn)
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
            AudioPlayer.PlaySound2D(Sound.card_place_deny);
            return;
        }
        
        AudioPlayer.PlaySound2D(Sound.card_place_down);

        // 1. Visually display the card's range on the board based on mouse position
        board.ShowRange(board.GetPlayerTile(), card.cardData.minRange, card.cardData.maxRange);

        

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
        AudioPlayer.PlaySound2D(Sound.card_place_down);

        // 5. Assign damage to any enemies on the tile
        HandleAttack(tile);

        // 6. Discard the card
        cardBeingPlayed.MoveToDiscardPile();

        // 7. End the player's turn if their action points are now 0
        if (player.actionPoints <= 0)
        {
            player.EndTurn(); 
        }

        // 8. Reset the board
        board.DisableShowRange();

        // 9. Unsubscribe from the tile click event
        Tile.OnTileClicked -= ResolveCard;

        // 10. Clean up
        cardBeingPlayed = null;
    }

   
    private void HandleAttack(Tile targetTile)
    {
        Tile playerTile = board.GetPlayerTile();

        bool isInOptimalRange =
                    board.TileIsInOptimalRange(playerTile, targetTile, cardBeingPlayed.cardData.minRange, cardBeingPlayed.cardData.maxRange);

        // === Damage Calculation ===
        //
        // if player is outside of optimal range for the selected card, 1 damage only
        int damage = isInOptimalRange ? cardBeingPlayed.cardData.damage : 1;

        float animationReleaseTime = 0.5f;

        player.AnimateAttack(targetTile);

        if (targetTile.Occupier != null)
        {
           

            if (targetTile.Occupier.OccupierTransform.TryGetComponent(out Enemy enemy))
            {
                
                if (isInOptimalRange)
                {
                    //knock back check
                    if(cardBeingPlayed.cardData.pushDistance !=0)
                    {
                        for(int i = 0; i < cardBeingPlayed.cardData.pushDistance; i++)
                        {
                            StartCoroutine(enemy.Movement(false));
                        }
                    }
                    switch(cardBeingPlayed.cardData.damageVariance)
                    {
                        // Returns a random value between the damage value +- the variance value
                        case EDamageVariance.Random:
                            damage = (int)Mathf.Round(UnityEngine.Random.Range((float)cardBeingPlayed.cardData.damage - cardBeingPlayed.cardData.varianceValue, (float)cardBeingPlayed.cardData.damage + cardBeingPlayed.cardData.varianceValue));
                            break;

                        // Adds the variance value per tile away from the player (can add negatives)
                        case EDamageVariance.RangeToPlayer:
                            damage += (int)(board.GetDistance(playerTile, targetTile) * cardBeingPlayed.cardData.varianceValue);
                            break;

                        // Adds the variance value per tile away from the center of the AOE
                        case EDamageVariance.RangeToCenter:
                            damage += (int)((float)cardBeingPlayed.cardData.aoeArea * cardBeingPlayed.cardData.varianceValue);
                            break;

                        // Double damage on a critical hit (1 in 10)
                        case EDamageVariance.Critical:
                            damage = UnityEngine.Random.Range(0, 10) >= 9 ? cardBeingPlayed.cardData.damage * 2 : cardBeingPlayed.cardData.damage;
                            break;

                        // No variance
                        case EDamageVariance.None:
                        default:
                            break;
                    }
                    
                    switch (cardBeingPlayed.cardData.aoeType)
                    {
                        case AoEType.Circle:
                            
                            foreach (Tile neighbor in targetTile.neighbourTiles)
                            {
                                if (neighbor.Occupier != null && neighbor.Occupier.OccupierTransform.TryGetComponent(out Enemy aoeEnemy))
                                {
                                    Debug.Log($"AOE dealt {damage} damage to {aoeEnemy.name}.");
                                    
                                    this.Wait(animationReleaseTime, () =>
                                    {
                                        //Projectile.CreateProjectile(player.transform, targetTile);

                                        aoeEnemy.TakeDamage(damage);
                                    });
                                }
                            }
                            
                            break;
                        case AoEType.Cone:
                            break;
                        case AoEType.Cross:

                            break;
                        case AoEType.Line:
                            //Board.Instance.GetKnockBackTile(targetTile);
                            break;
                        case AoEType.Single:
                        default:
                            break;
                    }

                    Debug.Log($"Player dealt {damage} damage to {enemy.name}.");
                }

                AudioPlayer.PlaySound3D(Sound.weapon_throw, player.transform.position);
                AudioPlayer.PlaySound3D(Sound.attack_vocal, player.transform.position, 0.25f);
                this.Wait(animationReleaseTime, () =>
                {
                    Projectile.CreateProjectile(player.transform, targetTile);
                    
                    enemy.TakeDamage(damage);
                });
            }
        }
        else 
        {

            if (cardBeingPlayed.cardData.aoeType == AoEType.Circle)
            {
                foreach (Tile neighbor in targetTile.neighbourTiles)
                {
                    if (neighbor.Occupier != null && neighbor.Occupier.OccupierTransform.TryGetComponent(out Enemy aoeEnemy))
                    {
                        Debug.Log($"AOE dealt {damage} damage to {aoeEnemy.name}.");
                        this.Wait(animationReleaseTime, () =>
                        {
                            Projectile.CreateProjectile(player.transform, targetTile);

                            aoeEnemy.TakeDamage(damage);
                        });
                    }
                }
            }
        }
    }
}
