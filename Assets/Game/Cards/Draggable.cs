using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class allows an object with a Collider to be dragged around the screen
public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private float mouseZ;
    private bool isDragging;

    // Store the offset between the object and the mouse
    private void OnMouseDown()
    {
        Debug.Log("Mouse down");

        mouseZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        StartCoroutine(MoveToMouse());
    }

    // Override this method to handle the mouse up event
    private void OnMouseUp()
    {
        Debug.Log("Mouse up");  
        isDragging = false;
    }

    // Moves the object to the mouse position
    private IEnumerator MoveToMouse()
    {
        while (isDragging)
        {
            transform.position = Vector3.Slerp(transform.position, GetMouseWorldPos() + offset, 0.25f);
            yield return null;
        }
        transform.position = GetMouseWorldPos() + offset;
    }

    // Get the mouse position in the world
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mouseZ;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
