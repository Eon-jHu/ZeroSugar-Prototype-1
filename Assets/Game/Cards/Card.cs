using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private string CardName;
    [SerializeField]
    private int Damage;
    [SerializeField]
    private int ActionCost;
    [SerializeField]
    private float Range;

    [SerializeField]
    private Sprite CardFace;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Method to tell all of its listeners to execute
    public void ExecuteEffect()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
