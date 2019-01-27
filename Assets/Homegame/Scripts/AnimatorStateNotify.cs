using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorStateNotify : StateMachineBehaviour
{
    public string OnEnterMessage;
    public string OnExitMessage;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var animatorInfo = new AnimatorEventInfo();
        animatorInfo.animator = animator;
        animatorInfo.stateInfo = stateInfo;
        animatorInfo.layerIndex = layerIndex;
        animatorInfo.newState = AnimatorEventInfo.StateChange.Enter;
        animatorInfo.message = OnEnterMessage;

        var recievers = animator.GetComponentsInParent<IAnimatorStateNotifyReciever>();
        
        foreach(var reciever in recievers)
        {
            reciever.OnStateChanged(animatorInfo);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var animatorInfo = new AnimatorEventInfo();
        animatorInfo.animator = animator;
        animatorInfo.stateInfo = stateInfo;
        animatorInfo.layerIndex = layerIndex;
        animatorInfo.newState = AnimatorEventInfo.StateChange.Exit;
        animatorInfo.message = OnExitMessage;

        var recievers = animator.GetComponentsInParent<IAnimatorStateNotifyReciever>();
        
        foreach(var reciever in recievers)
        {
            reciever.OnStateChanged(animatorInfo);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

public struct AnimatorEventInfo
{
    public Animator animator;
    public AnimatorStateInfo stateInfo;
    public int layerIndex;
    public enum StateChange
    {
        Enter,
        Exit,
    }
    public StateChange newState;
    public string message;
}

public interface IAnimatorStateNotifyReciever
{
    void OnStateChanged(AnimatorEventInfo eventInfo);
}