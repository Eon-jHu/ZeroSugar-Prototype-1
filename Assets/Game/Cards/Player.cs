using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int Health;

    [SerializeField]
    private int ActionPoints;

    //Temporary variables
    Card[] Deck;
    Card[] Played;
    Card[] Discarded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
