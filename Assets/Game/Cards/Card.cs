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
    // Boolean to check if the card is being inspected
    private bool isInspecting;
    // Boolean to check if the card is being dragged
    private bool isDragging;

    // Start is called before the first frame update
    private void Start()
    {
        cmRef = FindObjectOfType<CardManager>();
        cardCollider = GetComponent<BoxCollider>();
        cardRenderer = GetComponent<SpriteRenderer>();
        cardRenderer.sprite = cardData.cardSprite;

        inPlayZone = false;
        isInspecting = false;
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

    // ==================== CARD MOUSE OVER ====================

    private void OnMouseOver()
    {
        if (isInspecting) return;

        isInspecting = true;
        GetComponent<Renderer>().sortingOrder = 100;
        StartCoroutine(Inspect());
    }

    private IEnumerator Inspect()
    {
        while (transform.localScale.x < 0.75f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), 0.2f);
            transform.position += new Vector3(0, 0.5f, 0);
            yield return null;
        }
    }

    private void OnMouseExit()
    {
        if (!isInspecting) return;

        isInspecting = false;
        GetComponent<Renderer>().sortingOrder = handIndex;
        StartCoroutine(ResetInspect());

        // Only reset pos if it's not being dragged (since it will reset if it was)
        if (!isDragging)
        {
            StartCoroutine(ResetPos());
        }   
    }

    private IEnumerator ResetInspect()
    {
        while (transform.localScale.x > 0.25f)
        {
            transform.localScale = Vector3.Lerp(new Vector3(0.75f, 0.75f, 0.75f), new Vector3(0.25f, 0.25f, 0.25f), 0.2f);
            yield return null;
        }
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
