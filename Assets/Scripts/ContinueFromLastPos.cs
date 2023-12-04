using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueFromLastPos : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.RotateAround(animator.gameObject.transform.position, animator.gameObject.transform.up, 90f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         animator.gameObject.transform.RotateAround(animator.gameObject.transform.position, animator.gameObject.transform.up, animator.gameObject.transform.GetChild(48).transform.rotation.y);
        //Debug.Log(transform.rotation.y);   
        animator.gameObject.transform.GetChild(48).transform.rotation = Quaternion.Euler(animator.gameObject.transform.GetChild(48).transform.eulerAngles.x, 0, animator.gameObject.transform.GetChild(48).transform.eulerAngles.z);
        //animator.gameObject.transform.RotateAround(animator.gameObject.transform.position, animator.gameObject.transform.up, 90f);
    }*/

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
        //Debug.Log(animator.gameObject.transform.GetChild(48));
       

        //animator.gameObject.transform.Rotate(0, Time.deltaTime * 10, 0, Space.Self);*/
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
