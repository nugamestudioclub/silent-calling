using UnityEngine;
using UnityEngine.InputSystem;

// Add DontDestroyOnLoad? We'll see

// This is the core of the input system.
// It interacts with the Input Component, so you shouldn't access that directly.
// Instead, bind to the c# delegates that get called.
public class InputNexusScript : MonoBehaviour
{
    // delegate declaration
    public delegate void InputSystemDelegate(InputAction.CallbackContext context);
    public delegate void EmptyBindDelegate();

    // instance of delegate declarations
    // these are made into Events so that other classes cannot reset their values.
    public event InputSystemDelegate LateralBind, ButtonBind, UseBind, BackBind, MouseDeltaBind, MouseClickBind, ExtraBind;
    public event EmptyBindDelegate EmptyBind;

    #region Input Component Hooks
    // for all of these binds, if the delegate is empty, calls EmptyBind().

    // Called upon a Lateral input (wasd)
    public void OnLateralInput(InputAction.CallbackContext context)
    {
        if (LateralBind != null)
        {
            LateralBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon a Button input (space)
    public void OnButtonInput(InputAction.CallbackContext context)
    {
        if (ButtonBind != null)
        {
            ButtonBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon a Button input (J)
    public void OnUseBind(InputAction.CallbackContext context)
    {
        if (UseBind != null)
        {
            UseBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon a Button input (Shift)
    public void OnBackBind(InputAction.CallbackContext context)
    {
        if (BackBind != null)
        {
            BackBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon Mouse Delta (Vector2)
    public void OnMouseDeltaBind(InputAction.CallbackContext context)
    {
        if (MouseDeltaBind != null)
        {
            MouseDeltaBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon left mouse click.
    public void OnMouseClickBind(InputAction.CallbackContext context)
    {
        if (MouseClickBind != null)
        {
            MouseClickBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    // Called upon Q andor E pressed
    public void OnOneDimensionBind(InputAction.CallbackContext context)
    {
        if (ExtraBind != null)
        {
            ExtraBind.Invoke(context);

            return;
        }

        // OnEmptyBind(); not needed because most inputs don't use these keys
    }

    #endregion

    /// <summary>
    /// OnEmptyBind
    /// called when a bind doesn't do anything (should never be the case)
    /// if this function does get called, that means that nothing is listening
    /// to the input, which is bad. OnEmptyBind calls a special invoke that
    /// is never emptied until the end of the program.
    ///
    /// FIX THIS, RIGHT NOW IF MULTIPLE THINGS ARE HOOKED INTO EMPTYBIND THEY
    /// CAUSE A MEMORY LEAK BC THEY GET ADDED OVER AND OVER
    /// </summary>
    public void OnEmptyBind()
    {
        LateralBind = ButtonBind = UseBind = BackBind = MouseDeltaBind = MouseClickBind = ExtraBind = null;

        // this exists to let me know if the bind is empty
#if UNITY_EDITOR
        if (EmptyBind != null)
        {
            Debug.Log("Invoking emptybind.");

            EmptyBind.Invoke();

            return;
        }

        Debug.Log("EMPTYBIND IS EMPTY! fix it.");
#endif

        EmptyBind.Invoke();
    }
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
                InputNexusScript i = GameObject.FindGameObjectWithTag("InputNexus").GetComponent<InputNexusScript>();

                if (i == null)
                {
                    Debug.LogError("Nexus not found.");

                    return null;
                }

                ins = i;
            }

            return ins;
        }
    }

    // Declarations of functions

    protected abstract void Hook(); // hooks functions into inputs
    protected abstract void PersistentHook(); // hooks a function into the EmptyBind
    protected abstract void Free(); // frees functions from the inputs

    // Frees the function from the persistent hook.
    // Only call this from OnDestroy. this is not to be called in OnDisable.
    protected abstract void FreePersistentHook();
}
