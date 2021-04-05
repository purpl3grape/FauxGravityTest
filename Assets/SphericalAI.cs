using UnityEngine;
using System.Linq;

public class SphericalAI : MonoBehaviour
{
    private Vector3 _gravityUp;
    private GameObject[] Waypoints;
    private Transform NextWaypoint;
    [Header("Var Cache")]
    public Transform tr;
    public Transform PlanetTransform;
    public bool useWayPointNavigation;
    public bool useAutoForwardMovement;
    [Header("AI Settings")]
    [Range(0.01f, 100f)] public float Speed = 10f;
    [Range(0.01f, 1f)] public float WayPointRotationSmoothing = 0.15f;
    [Range(0.01f, 10f)] public float NextWayPointDistanceThreshold = 2f;
    [Range(0.01f, 100f)] public float XZ_RotationFactor = 50f;
    [Header("IF useWayPointNavigation == false, for turning AI Manually")]
    [Range(0.01f, 100f)] public float Y_NormalRotationFactor = 50f;

    private void Awake()
    {
        Waypoints = GameObject.FindGameObjectsWithTag("WayPoint");
        Waypoints.OrderByDescending(x => x.transform.GetSiblingIndex());
    }

    void Update()
    {
        //Gravity
        _gravityUp = (PlanetTransform.position - tr.position).normalized * (PlanetTransform.localScale.x / 4);//Some Speed Scale for larger objects
        if (Vector3.Distance(PlanetTransform.position, tr.position) > (PlanetTransform.localScale.x / 2 + tr.localScale.y))
            tr.position += _gravityUp * Time.deltaTime;

        //X-Z Rotation
        Quaternion _targetRotation = Quaternion.FromToRotation(-tr.up, _gravityUp) * tr.rotation;
        tr.rotation = Quaternion.Slerp(tr.rotation, _targetRotation, XZ_RotationFactor * Time.deltaTime);

        RotateBicycle();
        //X-Z Inputs Movement
        if (Input.GetKey(KeyCode.W)) {
            if (!useAutoForwardMovement) { tr.position += tr.forward * Speed * Time.deltaTime; }
        }
        else if (Input.GetKey(KeyCode.S)) { tr.position -= tr.forward * Speed * Time.deltaTime; }
        else if (Input.GetKey(KeyCode.D)) { tr.position += tr.right * Speed * Time.deltaTime; }
        else if (Input.GetKey(KeyCode.A)) { tr.position -= tr.right * Speed * Time.deltaTime; }

        if(useAutoForwardMovement) tr.position += tr.forward * Speed * Time.deltaTime;
        if (useWayPointNavigation) return;

        //Local Y rotation Inputs
        if (Input.GetKey(KeyCode.Q))
            tr.localRotation *= Quaternion.Euler(0, -Y_NormalRotationFactor * Time.deltaTime, 0);
        else if (Input.GetKey(KeyCode.E))
            tr.localRotation *= Quaternion.Euler(0, Y_NormalRotationFactor * Time.deltaTime, 0);

    }

    private float DistanceToPlane;
    private Vector3 DistToWaypoint;
    private Vector3 PointOnPlane;
    private Quaternion q;
    private int waypointIndex = 0;

    private void RotateBicycle()
    {
        if (!useWayPointNavigation) return;

        NextWaypoint = Waypoints[waypointIndex].transform;
        DistToWaypoint = NextWaypoint.position - tr.position;
        if (DistToWaypoint.magnitude < NextWayPointDistanceThreshold)
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
        DistanceToPlane = Vector3.Dot(tr.up, DistToWaypoint);
        PointOnPlane = NextWaypoint.position - (tr.up * DistanceToPlane);
        q = Quaternion.LookRotation(PointOnPlane - tr.position, tr.up);

        tr.localRotation = Quaternion.Slerp(tr.rotation, q, WayPointRotationSmoothing);
    }

    [ContextMenu("SphericalAI: Reset Default Settings")]
    private void SetDefaultSettings()
    {
        Speed = 10f;
        WayPointRotationSmoothing = 0.15f;
        NextWayPointDistanceThreshold = 2f;
        XZ_RotationFactor = 50f;
        Y_NormalRotationFactor = 50f;
        useWayPointNavigation = true;
        useAutoForwardMovement = true;
    }
}