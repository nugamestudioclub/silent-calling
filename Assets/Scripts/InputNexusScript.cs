using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputNexusScript : MonoBehaviour
{
    public delegate void InputSystemDelegate(InputAction.CallbackContext context);

    // static bc im lazy
    public static InputSystemDelegate LateralBind, ButtonBind, UseBind, BackBind;

    public void OnLateralInput(InputAction.CallbackContext context)
    {
        LateralBind?.Invoke(context);
    }

    public void OnButtonInput(InputAction.CallbackContext context)
    {
        ButtonBind?.Invoke(context);
    }

    public void OnUseBind(InputAction.CallbackContext context)
    {
        UseBind?.Invoke(context);
    }

    public void OnBackBind(InputAction.CallbackContext context)
    {
        BackBind?.Invoke(context);
    }

    public static void DesubscribeAll()
    {
        // this allows garbage collector to delete the contents of the delegates
        LateralBind = ButtonBind = UseBind = BackBind = null;
    }
}

public interface IPossessable
{
    protected abstract void Hook();
    protected abstract void Free();
}
