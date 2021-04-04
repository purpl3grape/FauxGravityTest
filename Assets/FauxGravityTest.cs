using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityTest : MonoBehaviour
{
    public Transform attractor;
    public Vector3 _gravityUp;
    public GameObject[] Waypoints;

    private Transform NextWaypoint;
    private void Awake()
    {
        Waypoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }

    void Update()
    {
        //Gravity
        _gravityUp = (attractor.position - transform.position).normalized * (attractor.localScale.x / 4);//Some Speed Scale for larger objects
        if (Vector3.Distance(attractor.position, transform.position) > (attractor.localScale.x / 2 + transform.localScale.y))
            transform.position += _gravityUp * Time.deltaTime;
            
        //X-Z Rotation
        Quaternion _targetRotation = Quaternion.FromToRotation(-transform.up, _gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, 50 * Time.deltaTime);

        RotateBicycle();
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
            transform.localRotation *= Quaternion.Euler(0, 10, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.localRotation *= Quaternion.Euler(0, -10, 0);
        }

    }

    float DistanceToPlane;
    Vector3 DistToWaypoint;
    Vector3 PointOnPlane;
    Quaternion q;
    int waypointIndex = 0;
    private void RotateBicycle()
    {
        NextWaypoint = Waypoints[waypointIndex].transform;
        DistToWaypoint = NextWaypoint.position - transform.position;
        if (DistToWaypoint.magnitude < 2)
        {
            if (waypointIndex == Waypoints.Length -1)
            {
                waypointIndex = 0;
                Debug.Log("Starting Waypoint");
            }
            else
            {
                waypointIndex++;
                Debug.Log("Next Waypoint");
            }
        }
        DistanceToPlane = Vector3.Dot(transform.up, DistToWaypoint);
        PointOnPlane = NextWaypoint.position - (transform.up * DistanceToPlane);
        q = Quaternion.LookRotation(PointOnPlane - transform.position, transform.up);

        transform.localRotation = Quaternion.Slerp(transform.rotation, q, 0.2f);
    }
}
