using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class allows an object with a Collider to be dragged around the screen
public abstract class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private float mouseZ;
    private bool isMouseDown;

    // Store the offset between the object and the mouse
    virtual protected void OnMouseDown()
    {
        Debug.Log("Mouse down");

        mouseZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = transform.position - GetMouseWorldPos();
        isMouseDown = true;
        StartCoroutine(MoveToMouse());
    }

    // Override this method to handle the mouse drag event
    protected virtual void OnMouseDrag() {}

    // Override this method to handle the mouse up event
    protected virtual void OnMouseUp()
    {
        Debug.Log("Mouse up");  
        isMouseDown = false;
    }

    // Moves the object to the mouse position
    IEnumerator MoveToMouse()
    {
        while (isMouseDown && (transform.position - GetMouseWorldPos() + offset).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, GetMouseWorldPos() + offset, 0.1f);
            yield return null;
        }
    }

    // Get the mouse position in the world
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mouseZ;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
