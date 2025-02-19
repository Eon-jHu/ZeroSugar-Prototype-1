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
    private GameManager gmRef;
    // Tracks if this is an active card
    private bool hasBeenPlayed;

    // Start is called before the first frame update
    void Start()
    {
        gmRef = FindObjectOfType<GameManager>();
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

    private void OnMouseDown()
    {
        if (!hasBeenPlayed)
        {
            transform.position += Vector3.up * 10;
            hasBeenPlayed = true;
            gmRef.availableCardSlots[handIndex] = true;
            Invoke("MoveToDiscardPile", 2f);
        }
    }

    void MoveToDiscardPile()
    {
        gmRef.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
