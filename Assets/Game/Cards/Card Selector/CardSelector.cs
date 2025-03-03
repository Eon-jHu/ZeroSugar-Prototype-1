using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CardSelector : Singleton<CardSelector>
{
    //public List<Card> SelectedCards = new();
    public List<CardData> AllCards;

    private CardData selectedCard;
    private CardData discardedCard;
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
        selectedCard = null;

        while (!selectedCard)
        {
            // code how to select random cards.
            List<CardData> randomCards = AllCards.OrderBy(_ => Random.value).Take(3).ToList();
            
            cardSelectorUI.BeginSelection(SelectCard, randomCards);
            
            yield return new WaitUntil(() => selectedCard);
        }
        
        CardManager cardManager = FindObjectOfType<CardManager>();
        discardedCard = null;

        // wait a period before bringing up discard interface.
        yield return new WaitForSecondsRealtime(0.5f);

        while (!discardedCard)
        {
            List<Card> discardCards = cardManager.deck.OrderBy(_ => Random.value).Take(3).ToList();
            
            cardSelectorUI.BeginSelection(DiscardCard, discardCards);
            
            yield return new WaitUntil(() => discardedCard);
        }

        CompleteCardSelection();
        Time.timeScale = 1;
    }

    private void SelectCard(CardData card)
    {
        // add card to deck.
        selectedCard = card;
        OnCardSelected?.Invoke(card);
    }

    private void DiscardCard(CardData card)
    {
        // remove card from deck.
        discardedCard = card;
    }

    private void CompleteCardSelection()
    {
        CardManager cardManager = FindObjectOfType<CardManager>();

        // handle discarding the chosen discard card.
        Card discardCard = cardManager.deck.FirstOrDefault(c => c.cardData == discardedCard);
        if (discardCard)
        {
            Destroy(discardCard.gameObject);
        }
        
        // handle adding the selected card.
        Transform cardHand = GameObject.FindWithTag("CardHand").transform;
        if (cardHand)
        {
            Card card = Instantiate(cardPrefab, cardHand);
            card.cardData = selectedCard;
            cardManager.deck.Add(card);
        }
        else
        {
            Debug.LogError("Cannot find Cards Parent. It should be tagged with 'Card Hand'");
        }
        
        SceneManager.UnloadSceneAsync("Card Select");
    }
}
