using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;

    // Draws a single card from the deck to the hand
    public void DrawCards()
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

                    drawnCard.transform.position = cardSlots[i].transform.position;

                    availableCardSlots[i] = false;
                    deck.Remove(drawnCard);
                    return;
                }
            }
        }        
    }

    public void Reshuffle()
    {
        if (discardPile.Count >= 1)
        {
            foreach (Card card in discardPile)
            {
                card.ResetPlayState();
                deck.Add(card);
            }
            discardPile.Clear();
        }

        ShuffleDeck();
    }

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
    }
}
