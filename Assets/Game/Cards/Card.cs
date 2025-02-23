using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : Draggable
{
    // Where the card is in our hand; also the order in the sorting layer
    public int handIndex;

    [SerializeField]
    private string cardName;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int actionCost;
    [SerializeField]
    private float range;

    // Sprite for the card face
    [SerializeField]
    private Sprite cardSprite;

    // GameManager reference
    private CardManager cmRef;

    // Tracks if this is an active card (UNUSED)
    // private bool hasBeenPlayed;

    // Own collider
    private BoxCollider cardCollider;
    // Renderer for the card
    private SpriteRenderer cardRenderer;
    // Booleans to check if it's in the play zone or not
    private bool inPlayZone;

    // Start is called before the first frame update
    void Start()
    {
        cmRef = FindObjectOfType<CardManager>();
        cardCollider = GetComponent<BoxCollider>();
        cardRenderer = GetComponent<SpriteRenderer>();
        inPlayZone = false;
    }

    // ================= COLLISION AND MOUSE UP =================

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetComponent<Renderer>().sortingOrder = 100;
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        cardRenderer.sortingOrder = handIndex;

        if (inPlayZone)
        {
            // Play the card
            cmRef.availableCardSlots[handIndex] = true;
            //Invoke("MoveToDiscardPile", 2f);
            MoveToDiscardPile();
        }
        else
        {
            // Reset its position
            StartCoroutine(ResetPos());
        }
    }

    IEnumerator ResetPos()
    {
        while ((transform.position - cmRef.cardSlots[handIndex].position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, cmRef.cardSlots[handIndex].position, 0.1f);
            yield return null;
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayZone"))
        {
            inPlayZone = true;
            Debug.Log("In play zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayZone"))
        {
            inPlayZone = false;
            Debug.Log("Left play zone");
        }
    }

    // =========================================================

    void MoveToDiscardPile()
    {
        cmRef.discardPile.Add(this);
        gameObject.SetActive(false);
        inPlayZone = false; // reset played status
    }
}
