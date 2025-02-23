using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class allows an object with a Collider to be dragged around the screen
public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 previousMovement;
    private float mouseZ;

    // Store the offset between the object and the mouse
    private void OnMouseDown()
    {
        mouseZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    // Move the object to follow the mouse position
    private void OnMouseDrag()
    {
        Vector3 targetPos = GetMouseWorldPos() + offset + previousMovement;
        Vector3 movement = (targetPos - transform.position) / 10;
        transform.position += movement;
        previousMovement = movement;
    }

    // Get the mouse position in the world
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mouseZ;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
