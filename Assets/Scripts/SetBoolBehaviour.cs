using UnityEngine;
using UnityEngine.Animations;

public class SetBoolBehaviour : StateMachineBehaviour
{
    public string boolName;
    public bool updateOnStateEnter;
    public bool updateOnStateExit;
    public bool updateOnStateMachineEnter;
    public bool updateOnStateMachineExit;
    public bool valueOnEnter;
    public bool valueOnExit;

    private bool HasParameter(Animator animator, string paramName)
    {
        if (string.IsNullOrEmpty(paramName)) return false;
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool) return true;
        }
        return false;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnStateEnter && HasParameter(animator, boolName))
        {
            animator.SetBool(boolName, valueOnEnter);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnStateExit && HasParameter(animator, boolName))
        {
            animator.SetBool(boolName, valueOnExit);
        }
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachineEnter && HasParameter(animator, boolName))
        {
            animator.SetBool(boolName, valueOnEnter);
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachineExit && HasParameter(animator, boolName))
        {
            animator.SetBool(boolName, valueOnExit);
        }
    }
}