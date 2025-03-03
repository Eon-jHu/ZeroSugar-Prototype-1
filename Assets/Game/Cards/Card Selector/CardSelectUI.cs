using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectUI : MonoBehaviour
{
    private CardData card;
    
    [SerializeField] private TMP_Text title;
    [SerializeField] private Image iconImage;
    
    public void SetCardSelect(CardData cardSelectionOption)
    {
        CardData data = cardSelectionOption;
        
        title.text = data.cardName;
        iconImage.sprite = data.cardSprite;
        card = cardSelectionOption;
    }

    public void OnMousePressed()
    {
        CardSelectorUI cardSelectorUI = transform.GetComponentInParent<CardSelectorUI>();
        
        cardSelectorUI.SelectCard(card);
    }
    
    public void OnMouseHover()
    {
        transform.localScale *= 1.05f;
    }

    public void OnMouseLeave()
    {
        transform.localScale = Vector3.one;
    }
}
