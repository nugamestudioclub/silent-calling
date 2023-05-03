using UnityEngine;
using UnityEngine.InputSystem;

// This is the core of the input system.
// It interacts with the Input Component, so you shouldn't access that directly.
// Instead, bind to the c# delegates that get called.
public class InputNexusScript : MonoBehaviour
{
    // delegate declaration
    public delegate void InputSystemDelegate(InputAction.CallbackContext context);

    // instance of delegate declarations
    // these are made into Events so that other classes cannot reset their values.
    public event InputSystemDelegate LateralBind, ButtonBind, UseBind, BackBind, MouseDeltaBind, MouseClickBind, ExtraBind;

    private void Awake()
    {
        GameObject[] other = GameObject.FindGameObjectsWithTag("InputNexus");
        
        if (other.Length > 1)
        {
            Debug.LogWarning("Dupe nexus detected, deleting " + name);

            Destroy(gameObject);
        }

        // Debug.Log("DontDestroyOnLoad registered for " + name);

        // DontDestroyOnLoad(gameObject);
    }

    #region Input Component Hooks
    // for all of these binds, if the delegate is empty, calls EmptyBind().

    // Called upon a Lateral input (wasd)
    public void OnLateralInput(InputAction.CallbackContext context)
    {
       LateralBind?.Invoke(context);
    }

    // Called upon a Button input (space)
    public void OnButtonInput(InputAction.CallbackContext context)
    {
        ButtonBind?.Invoke(context);
    }

    // Called upon a Button input (J)
    public void OnUseBind(InputAction.CallbackContext context)
    {
        UseBind?.Invoke(context);
    }

    // Called upon a Button input (Shift)
    public void OnBackBind(InputAction.CallbackContext context)
    {
        BackBind?.Invoke(context);
    }

    // Called upon Mouse Delta (Vector2)
    public void OnMouseDeltaBind(InputAction.CallbackContext context)
    {
        MouseDeltaBind?.Invoke(context);
    }

    // Called upon left mouse click.
    public void OnMouseClickBind(InputAction.CallbackContext context)
    {
        MouseClickBind?.Invoke(context);
    }

    // Called upon Q andor E pressed
    public void OnOneDimensionBind(InputAction.CallbackContext context)
    {
        ExtraBind?.Invoke(context);
    }

    #endregion
}

// an abstract class that all input-related objects should interit from.
public abstract class PossessableObject : MonoBehaviour
{
    // Lazy-loading the component is nice.
    private InputNexusScript ins;
    protected InputNexusScript INS
    {
        get
        {
            if (ins == null)
            {
                GameObject g = GameObject.FindGameObjectWithTag("InputNexus");
                
                if (g == null)
                {
                    Debug.LogError("Nexus not found.");

                    return null;
                }

                InputNexusScript i = g.GetComponent<InputNexusScript>();

                ins = i;
            }

            return ins;
        }
    }

    protected virtual void OnEnable()
    {
        Hook();
    }

    protected virtual void OnDisable()
    {
        Free();
    }

    // Declarations of functions

    protected abstract void Hook(); // hooks functions into inputs
    protected abstract void Free(); // frees functions from the inputs
}
