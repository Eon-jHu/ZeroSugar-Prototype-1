using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IOccupier
{
    public enum eEnemyType
    {
        MELEE,
        RANGED,
    }

    [SerializeField]
    private int Health;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
