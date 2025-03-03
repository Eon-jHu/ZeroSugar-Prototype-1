using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;
    public BoxCollider playArea;

    [SerializeField]
    public GameObject drawButton;

    private bool hasDrawnFirstDeck = false;
    public bool startOfTurn = true;

    private AudioSource cardSFXSource;
    [SerializeField] private AudioClip drawCardClip;

    private void Start()
    {
        foreach (Transform slot in cardSlots)
        {
            slot.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }


    }

    // Draws cards from the deck so it fills the hand
    public void DrawHand()
    { 
        while (DrawCards())
        {
            continue;
        }
        
        AudioPlayer.PlaySound2D(Sound.card_draw);
        
        if (!hasDrawnFirstDeck)
        {
            hasDrawnFirstDeck = true;
            //Player.Instance.EndTurn();
        }
        startOfTurn = false;
    }

    // Draws a single card from the deck to the hand
    public bool DrawCards()
    {
        if (deck.Count > 0)
        {
            // Pick the top card of the deck
            Card drawnCard = deck[^1];

            // Place it in a card slot
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (availableCardSlots[i])
                {
                    // Display the card and move it into position
                    drawnCard.gameObject.SetActive(true);
                    drawnCard.handIndex = i;
                    drawnCard.GetComponent<Renderer>().sortingOrder = i; // sets the sorting order

                    drawnCard.transform.SetPositionAndRotation(cardSlots[i].transform.position, cardSlots[i].transform.rotation);
                    
                    availableCardSlots[i] = false;
                    deck.Remove(drawnCard);
                    return true;
                }
            }
            // Returns false after looping through all card slots
            return false;
        }
        else
        {
            Reshuffle();
            DrawCards();
        }
        return false;
    }

    // Shuffles the discard pile back into the deck
    public void Reshuffle()
    {
        if (discardPile.Count >= 1)
        {
            foreach (Card card in discardPile)
            {
                deck.Add(card);
            }
            discardPile.Clear();
        }

        ShuffleDeck();

    }
    
    // Shuffles the deck
    public void ShuffleDeck()
    {
        int count = deck.Count;
        if (count <= 0) return;

        for (int i = 0; i < count - 1; i++)
        {
            var r = UnityEngine.Random.Range(i, count);
            (deck[r], deck[i]) = (deck[i], deck[r]);
        }
    }

    private void Update()
    {
        deckSizeText.text = deck.Count.ToString();
        
        //if (startOfTurn == false)
        //{
        //    drawButton.SetActive(false);
        //}
        //else
        //{
        //    drawButton.SetActive(true);
        //}
    }
}
