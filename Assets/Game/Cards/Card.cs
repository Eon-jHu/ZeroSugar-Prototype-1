using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Where the card is in our hand
    public int handIndex;

    [SerializeField]
    private string cardName;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int actionCost;
    [SerializeField]
    private float range;

    [SerializeField]
    private Sprite cardFace;

    // GameManager reference
    private CardManager cmRef;
    // Tracks if this is an active card
    private bool hasBeenPlayed;
    // Own collider
    private BoxCollider cardCollider;

    // Start is called before the first frame update
    void Start()
    {
        cmRef = FindObjectOfType<CardManager>();
        cardCollider = GetComponent<BoxCollider>();
        hasBeenPlayed = false;
    }

    // Method to tell all of its listeners to execute
    public void ExecuteEffect()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayState()
    {
        hasBeenPlayed = false;
    }

    private void OnMouseUpAsButton()
    {
        if (hasBeenPlayed) return;

        if (cardCollider.bounds.Intersects(cmRef.playArea.bounds))
        {
            hasBeenPlayed = true;
            cmRef.availableCardSlots[handIndex] = true;
            //Invoke("MoveToDiscardPile", 2f);
            MoveToDiscardPile();
        }
    }

    void MoveToDiscardPile()
    {
        cmRef.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
