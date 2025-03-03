using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
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
        isDragging = false;
    }

    // ================= COLLISION AND MOUSE UP =================

    private void OnMouseDown()
    {
        isDragging = true;
        GetComponent<Renderer>().sortingOrder = 100;
        AudioPlayer.PlaySound2D(Sound.card_select);
    }

    private void OnMouseUp()
    {
        isDragging = false;
        cardRenderer.sortingOrder = handIndex;

        if (inPlayZone)
        {
            //Invoke("MoveToDiscardPile", 2f);
            StartCoroutine(FadeOutAnim());

            // Disable colliders
            cardCollider.enabled = false;
            cmRef.playArea.enabled = false;

            GameManager.Instance.PlayCard(this);
        }
        else
        {
            // Reset its position
            StartCoroutine(ResetCard());
        }
    }

    private IEnumerator FadeOutAnim()
    {
        while (cardRenderer.color.a > 0.2f)
        {
            cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, cardRenderer.color.a - 0.2f);
            yield return null;
        }
        cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, 0);
    }

    private IEnumerator FadeInAnim()
    {
        while (cardRenderer.color.a < 0.8f)
        {
            cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, cardRenderer.color.a + 0.2f);
            yield return null;
        }
        cardRenderer.color = new Color(cardRenderer.color.r, cardRenderer.color.g, cardRenderer.color.b, 1);
    }


    public void ResetCardInHand()
    {
        // Stop relevant Coroutines
        StopAllCoroutines();

        StartCoroutine(ResetCard());
        StartCoroutine(FadeInAnim());
        inPlayZone = false;
        cardCollider.enabled = true;
        cmRef.playArea.enabled = true;
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
        StopAllCoroutines();
        if (!inPlayZone)
        {
            StartCoroutine(FadeInAnim()); // cheeky fix
        }
        StartCoroutine(InspectCard());
        GetComponent<Renderer>().sortingOrder = 100;
    }

    private void OnMouseExit()
    {
        if (!isInspecting) return;
        
        isInspecting = false;
        StopAllCoroutines();
        if (!inPlayZone)
        {
            StartCoroutine(FadeInAnim()); // cheeky fix
        }
        if (!inPlayZone || !isDragging)
        {
            StartCoroutine(ResetCard());
        }
        GetComponent<Renderer>().sortingOrder = handIndex;
    }

    private IEnumerator InspectCard()
    {
        Vector3 targetScale = new(0.75f, 0.75f, 0.75f);
        Vector3 targetPosition = new(cmRef.cardSlots[handIndex].position.x, cmRef.cardSlots[handIndex].position.y + 3.1f, cmRef.cardSlots[handIndex].position.z);

        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f || Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.333f);
            if (!isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.333f);
            }
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPosition;
    }

    private IEnumerator ResetCard()
    {
        Vector3 originalScale = new(0.5f, 0.5f, 0.5f);
        Vector3 originalPosition = cmRef.cardSlots[handIndex].position;

        while (Vector3.Distance(transform.localScale, originalScale) > 0.01f || Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, 0.333f);

            if (!isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, originalPosition, 0.333f);
            }
            yield return null;
        }

        transform.localScale = originalScale;
        transform.position = originalPosition;
    }

    // =========================================================

    public void MoveToDiscardPile()
    {
        // No longer in the hand
        cmRef.availableCardSlots[handIndex] = true;

        cmRef.discardPile.Add(this);
        gameObject.SetActive(false);
        inPlayZone = false; // reset played status
    }
}
