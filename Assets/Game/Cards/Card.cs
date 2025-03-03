using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : Draggable
{
    // Card data in a separate scriptable object
    public CardData cardData;

    // Where the card is in our hand; also the order in the sorting layer
    public int handIndex;

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
    private void Start()
    {
        cmRef = FindObjectOfType<CardManager>();
        cardCollider = GetComponent<BoxCollider>();
        cardRenderer = GetComponent<SpriteRenderer>();
        cardRenderer.sprite = cardData.cardSprite;

        inPlayZone = false;
    }

    // ================= COLLISION AND MOUSE UP =================

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetComponent<Renderer>().sortingOrder = 100;
        AudioPlayer.PlaySound2D(Sound.card_select);
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        cardRenderer.sortingOrder = handIndex;

        if (inPlayZone)
        {
            //Invoke("MoveToDiscardPile", 2f);
            StartCoroutine(FadeOutAnim());
            cardCollider.enabled = false;
            GameManager.Instance.PlayCard(this);
        }
        else
        {
            // Reset its position
            StartCoroutine(ResetPos());
        }
    }

    private IEnumerator FadeOutAnim()
    {
        while (cardRenderer.color.a > 0)
        {
            cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, cardRenderer.color.a - 0.1f);
            yield return null;
        }
    }

    private IEnumerator FadeInAnim()
    {
        while (cardRenderer.color.a < 1)
        {
            cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, cardRenderer.color.a + 0.1f);
            yield return null;
        }
    }

    private IEnumerator ResetPos()
    {
        while ((transform.position - cmRef.cardSlots[handIndex].position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, cmRef.cardSlots[handIndex].position, 0.1f);
            yield return null;
        }        
    }

    public void ResetCardInHand()
    {
        StartCoroutine(ResetPos());
        StartCoroutine(FadeInAnim());
        cardCollider.enabled = true;
        inPlayZone = false;
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

    private void MoveToPlayedArea()
    {

    }

    public void MoveToDiscardPile()
    {
        // No longer in the hand
        cmRef.availableCardSlots[handIndex] = true;

        cmRef.discardPile.Add(this);
        gameObject.SetActive(false);
        inPlayZone = false; // reset played status
    }
}
