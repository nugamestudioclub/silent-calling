using UnityEngine;
using System.Collections;

public class TwoAnimHandlerScript : MonoBehaviour
{
    // ahhh, modularity

    Animator _animator; // cache the animator, because GetComponent is expensive
    int _parameterHash; // hash the string name of the parameter, because int search is faster than string search
    int _blendHash;

    IEnumerator _coroutineInstance;

    float _RATE = 0.125f;

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
        _blendHash = Animator.StringToHash("Blend");

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
        CurrentState = (int)t.StateType;

        if (t.StateType == TwoState.Running)
        {
            _coroutineInstance = AnimateBlendTree();
            StartCoroutine(_coroutineInstance);
        }

        else if (_coroutineInstance != null)
        {
            StopCoroutine(_coroutineInstance);
            SetBlend(0f);
        }
    }

    IEnumerator AnimateBlendTree()
    {
        float v = 0f;

        while (v < 0.9f)
        {
            v = Mathf.Lerp(v, 1f, _RATE);

            SetBlend(v);

            yield return new WaitForFixedUpdate();
        }
    }

    void SetBlend(float value)
    {
        _animator.SetFloat(_blendHash, value);
    }

}
