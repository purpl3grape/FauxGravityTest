using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityTest : MonoBehaviour
{
    public Transform attractor;
    public Vector3 _gravityUp;

    void Update()
    {
        //Gravity
        _gravityUp = (attractor.position - transform.position).normalized * (attractor.localScale.x / 4);//Some Speed Scale for larger objects
        if (Vector3.Distance(attractor.position, transform.position) > (attractor.localScale.x/2  + transform.localScale.y))
            transform.position += _gravityUp * Time.deltaTime;

        //X-Z Rotation
        Quaternion _targetRotation = Quaternion.FromToRotation(-transform.up, _gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, 50 * Time.deltaTime);

        //(Local Y) Local Heading Rotation
        _targetRotation = Quaternion.Euler(transform.forward) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, 1 * Time.deltaTime);


        //X-Z Inputs Movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right;
        }

        //Local Y rotation Inputs
        if (Input.GetKey(KeyCode.Q))
        {
            transform.localRotation = Quaternion.Euler(0, 10, 0) * transform.localRotation;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.localRotation = Quaternion.Euler(0, -10, 0) * transform.localRotation;
        }


    }
}
