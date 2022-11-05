using UnityEngine;
using UnityEngine.InputSystem;

// DontDestroyOnLoad? We'll see.

public class InputNexusScript : MonoBehaviour
{
    public delegate void InputSystemDelegate(InputAction.CallbackContext context);
    public delegate void EmptyBindDelegate();

    public InputSystemDelegate LateralBind, ButtonBind, UseBind, BackBind;

    public EmptyBindDelegate EmptyBind;

    public void OnLateralInput(InputAction.CallbackContext context)
    {
        if (LateralBind != null)
        {
            LateralBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    public void OnButtonInput(InputAction.CallbackContext context)
    {
        if (ButtonBind != null)
        {
            ButtonBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    public void OnUseBind(InputAction.CallbackContext context)
    {
        if (UseBind != null)
        {
            UseBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    public void OnBackBind(InputAction.CallbackContext context)
    {
        if (BackBind != null)
        {
            BackBind.Invoke(context);

            return;
        }

        OnEmptyBind();
    }

    /// <summary>
    /// OnEmptyBind
    /// called when a bind doesn't do anything (should never be the case)
    /// if this function does get called, that means that nothing is listening
    /// to the input, which is bad. OnEmptyBind calls a special invoke that
    /// is never emptied until the end of the program.
    /// </summary>
    public void OnEmptyBind()
    {
        // this exists to let me know if the bind is empty
#if UNITY_EDITOR
        if (EmptyBind != null)
        {
            Debug.Log("Invoking EMPTYBIND.");

            EmptyBind.Invoke();

            return;
        }

        Debug.Log("EMPTYBIND IS EMPTY!");
#endif

        EmptyBind.Invoke();
    }

    // called by PossessableObjects that want to clear everything
    public void DesubscribeAll()
    {
        // this allows garbage collector to delete the contents of the delegates
        LateralBind = ButtonBind = UseBind = BackBind = null;
    }
}

public abstract class PossessableObject : MonoBehaviour
{
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

    protected abstract void Hook(); // hooks functions into inputs
    protected abstract void PersistentHook(); // hooks a function into the EmptyBind
    protected abstract void Free(); // frees functions from the inputs

    // Frees the function from the persistent hook.
    // Only call this from OnDestroy. this is not to be called in OnDisable.
    protected abstract void FreePersistentHook();

    // wipes all Binds; used if the PossessableObject is selfish and takes all input
    protected void DesubscribeAll()
    {
        // this allows garbage collector to delete the contents of the delegates
        INS.LateralBind = INS.ButtonBind = INS.UseBind = INS.BackBind = null;
    }
}
