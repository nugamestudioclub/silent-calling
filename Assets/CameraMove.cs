using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    private GameObject attachPoint;
    public float defaultZoom = 4.0f;
    public float height = 1.0f;
    public float smoothSpeed = 10.0f;

    [SerializeField]
    [Header("Radius of the sphere Raycast. This controls how close a camera" +
        "is to a blocking obstacle before moving in front of it.")]
    public float sphereRadius = 0.1f;

    public LayerMask layerMask;

    public Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        attachPoint = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //store the direction as a unit vector
        Quaternion cameraDirection = attachPoint.transform.rotation;

        Vector3 targetPosition = attachPoint.transform.position
            - attachPoint.transform.forward * defaultZoom
            + attachPoint.transform.up * height;

        RaycastHit hit;
        Debug.DrawRay(targetPosition, attachPoint.transform.position - targetPosition, Color.red, 0.1f);



        //default layer mask for now, this might change soon.
        if (Physics.SphereCast(targetPosition, 0.1f,
            attachPoint.transform.position - targetPosition, out hit, defaultZoom, layerMask))
        {
            targetPosition = hit.point;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
    }

    //TODO: add behavior to move camera in front of blocking objects

    /*
    float checkIfObjectsBlocking(Vector3 targetPosition)
    {
        

    }*/
}
