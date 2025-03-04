using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectorUI : MonoBehaviour
{
    // may need to change Card to CardData type
    [SerializeField] private Transform layoutParent;
    [SerializeField] private CardSelectUI cardSelectPrefab;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private TMP_Text selectorTitle;

    [SerializeField] private Button skipButton;
    
    private Action<CardData> OnCardClicked;
    public void BeginSelection(Action<CardData> callback, List<CardData> cards)
    {
        // should be a random 3 cards.
        selectorTitle.text = "Add a card to your deck";

        foreach (var card in cards)
        {
            CardSelectUI cardSelect = Instantiate(cardSelectPrefab, layoutParent);
            cardSelect.SetCardSelect(card);
        }

        StartCoroutine(CardSelectionRoutine(callback));
    }
    
    public void BeginSelection(Action<CardData> callback, List<Card> cards)
    {
        // should be a random 3 cards.
        selectorTitle.text = "<color=red>Discard</color> a card";
         
        foreach (var card in cards)
        {
            CardSelectUI cardSelect = Instantiate(cardSelectPrefab, layoutParent);
            cardSelect.SetCardSelect(card.cardData);
        }

        StartCoroutine(CardSelectionRoutine(callback));
    }

    private IEnumerator CardSelectionRoutine(Action<CardData> callback)
    {
        selectionPanel.SetActive(true);

        OnCardClicked += Option;
        bool isSelecting = true;
        Time.timeScale = 0;
        
        while (isSelecting) yield return null;

        void Option(CardData card)
        {
            Time.timeScale = 1;
            OnCardClicked -= Option;
            selectionPanel.SetActive(false);
            ClearCardSelection();
            
            callback?.Invoke(card);
            isSelecting = false;
            // player has committed to selecting a new card, hide the skip button as they must now choose a card to discard.
            skipButton.gameObject.SetActive(false);
        }
    }

    public void SelectCard(CardData card)
    {
        OnCardClicked?.Invoke(card);
    }

    public void SkipButtonPressed()
    {
        CardSelector cardSelector = FindObjectOfType<CardSelector>();

        if (cardSelector)
        {
            cardSelector.SkipCardSelection();
        }
    }


    private void ClearCardSelection()
    {
        foreach (var option in layoutParent.GetComponentsInChildren<CardSelectUI>(true))
        {
            Destroy(option.gameObject);
        }
    }
}
