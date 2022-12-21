using UnityEngine;

public class TwoAnimHandlerScript : MonoBehaviour
{
    // ahhh, modularity

    Animator _animator; // cache the animator, because GetComponent is expensive
    int _parameterHash; // hash the string name of the parameter, because int search is faster than string search

    int _currentState = 0; // the variable that stores the value of CurrentState
    private int CurrentState // the "visible" variable that we access to change the current state
    {
        set
        {
            if (value != _currentState) // if the incoming value is new...
            {
                _currentState = value; // ...set the current state to that value...
                _animator.SetInteger(_parameterHash, value); // ...and update the animator.
            }
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _parameterHash = Animator.StringToHash("State"); // getting the hash

        CurrentState = 0;
    }

    // both OnEnable and OnDisable deal with hooks for the StateChange event
    void OnEnable()
    {
        TwoSMScript tsms = GetComponent<TwoSMScript>();

        tsms.OnStateChanged += UpdateAnimator;
    }

    void OnDisable()
    {
        TwoSMScript tsms = GetComponent<TwoSMScript>();

        tsms.OnStateChanged -= UpdateAnimator; 
    }

    void UpdateAnimator(TwoBaseState t)
    {
        CurrentState = (int)t.StateType; // yup, that's the entire script for movement animation lmao
    }

}
