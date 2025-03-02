using UnityEngine;

public class IdleAnimatorState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("AttackBasic", false);
    }
}
