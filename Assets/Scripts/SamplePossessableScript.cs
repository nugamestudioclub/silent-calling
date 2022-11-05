using UnityEngine;
using UnityEngine.InputSystem; // if you have a PossessableObject, you need this line.


// a sample class to show the Input Nexus system.
public class SamplePossessableScript : PossessableObject
{
    void Start()
    {
        // we use persistentHook here to ensure that this is the "default"
        // hooked object. It won't be removed until execution stops.
        PersistentHook();

        //Hook();
    }

    // These are our functions that will do things with input.
    private void WithInputVector(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(string.Format("Lateral input: {0}", context.ReadValue<Vector2>()));
        }
    }

    private void WithButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);

            g.transform.position += Vector3.up * 3f;

            Rigidbody r = g.AddComponent<Rigidbody>();

            r.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    private void WithUseButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 random =
                new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f));

            Camera.main.transform.LookAt(random);
        }
    }

    private void WithBackButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Camera.main.transform.SetPositionAndRotation
                (Vector3.up + Vector3.back * 10f, Quaternion.identity);
        }
    }

    // how you implement the PossessableObject functions
    protected override void Hook()
    {
        // DesubscribeAll(); might be used, but in most cases don't do this.

        INS.LateralBind += WithInputVector;
        INS.ButtonBind += WithButton;
        INS.UseBind += WithUseButton;
        INS.BackBind += WithBackButton;
    }

    protected override void Free()
    {
        INS.LateralBind -= WithInputVector;
        INS.ButtonBind -= WithButton;
        INS.UseBind -= WithUseButton;
        INS.BackBind -= WithBackButton;
    }

    protected override void PersistentHook()
    {
        INS.EmptyBind += Hook; // ensures something is bound, always.
    }

    protected override void FreePersistentHook()
    {
        INS.EmptyBind -= Hook;
    }
}
