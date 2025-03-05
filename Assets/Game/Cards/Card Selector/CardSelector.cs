using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CardSelector : Singleton<CardSelector>
{
    public List<CardData> AllCards;
    
    [SerializeField] private Card cardPrefab;

    public static event Action<CardData> OnCardSelected;

    public void SelectCard()
    {
        // called at end of round.
        StartCoroutine(BeginCardSelection());
    }

    private IEnumerator BeginCardSelection()
    {
        Time.timeScale = 0;

        CardSelectorUI cardSelectorUI = FindObjectOfType<CardSelectorUI>();
        CardData selectedCard = null;

        while (!selectedCard)
        {
            // code how to select random cards.
            List<CardData> randomCards = AllCards.OrderBy(_ => Random.value).Take(3).ToList();
            
            cardSelectorUI.BeginSelection(card => selectedCard = card, randomCards);
            
            yield return new WaitUntil(() => selectedCard);
        }
        
        CardManager cardManager = FindObjectOfType<CardManager>();
        CardData discardCard = null;

        // wait a period before bringing up discard interface.
        yield return new WaitForSecondsRealtime(0.5f);

        while (!discardCard)
        {
            List<Card> discardCards = new List<Card>(FindObjectsOfType<Card>(true));
            // add the cards in the discard pile too
            discardCards = discardCards.OrderBy(_ => Random.value).Take(3).ToList();
            
            cardSelectorUI.BeginSelection(card => discardCard = card, discardCards);
            
            yield return new WaitUntil(() => discardCard);
        }

        CompleteCardSelection(selectedCard, discardCard);
        Time.timeScale = 1;
    }
    
    public void SkipCardSelection()
    {
        SceneManager.UnloadSceneAsync("Card Select");
        Time.timeScale = 1;
    }

    private void CompleteCardSelection(CardData selectedCard, CardData discardCard)
    {
        CardManager cardManager = FindObjectOfType<CardManager>();

        // handle discarding the chosen discard card.
        Card cardToDiscard = FindObjectsOfType<Card>(true).ToList().FirstOrDefault(c => c.cardData == discardCard);
        if (cardToDiscard)
        {
            cardManager.deck.Remove(cardToDiscard);
            cardManager.discardPile.Remove(cardToDiscard);
            
            // if the card is not in the deck and not in the discard pile, then it's in the hand, so reset the available card slot.
            if (!cardManager.deck.Contains(cardToDiscard) && !cardManager.discardPile.Contains(cardToDiscard))
                cardManager.availableCardSlots[cardToDiscard.handIndex] = true;
            
            Destroy(cardToDiscard.gameObject);
        }

        bool successAddingCard = AddNewCard();
        if (!successAddingCard)
        {
            Debug.LogError("Cannot find Cards Parent. It should be tagged with 'Card Hand'");
        }
        
        bool AddNewCard()
        {
            Transform cardHand = GameObject.FindWithTag("CardHand").transform;
            cardHand = !cardHand ? FindObjectOfType<Card>(true).transform.parent : cardHand;

            if (!cardHand)
                return false;

            Card card = Instantiate(cardPrefab, cardHand);
            card.cardData = selectedCard;
            cardManager.deck.Add(card);

            return true;
        }
        
        SceneManager.UnloadSceneAsync("Card Select");
    }
}
