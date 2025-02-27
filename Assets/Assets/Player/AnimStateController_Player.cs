using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStateController_Player : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if player presses w key; CHANGE THIS TO ATTACK CARD PLAYED
        if (Input.GetKey("w"))
        {
            //then play basic animation
            animator.SetBool("AttackBasic", true);
        }
    }
}
