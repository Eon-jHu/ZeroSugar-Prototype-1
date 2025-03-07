using UnityEngine;

public class IdleAnimatorState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("Attack", false);
       // will cause a warning, need to unify attack parameter names in the animators.
    }
}
