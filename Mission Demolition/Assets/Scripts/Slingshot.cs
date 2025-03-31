using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // fields set in the Unity Inspector
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    // Rubber band references
    public Transform bandLeft;
    public Transform bandRight;
    public LineRenderer rubberBand;

    // Trajectory line references
    public LineRenderer trajectoryLine;
    public int trajectoryPoints = 30;
    public float timeStep = 0.05f;


    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        // Setup rubber band references
        bandLeft = transform.Find("LeftArm/BandLeft");
        bandRight = transform.Find("RightArm/BandRight");
        rubberBand = GameObject.Find("RubberBand").GetComponent<LineRenderer>();
        rubberBand.enabled = false;

        // Setup trajectory line references
        trajectoryLine = GameObject.Find("TrajectoryLine").GetComponent<LineRenderer>();
        trajectoryLine.enabled = false;
    }

    private void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        aimingMode = true;

        // Instantiate the projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        // Enable the rubber band
        rubberBand.enabled = true;
        rubberBand.positionCount = 3;
    }

    private void Update()
    {
        if (!aimingMode) return;

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launch position to the mouse position
        Vector3 mouseDelta = mousePos3D - launchPos;

        // Limit mouseDelta to the radius of the slingshot
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile to new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // Update the trajectory line
        Vector3 velocity = -mouseDelta * velocityMult;
        DisplayTrajectory(projPos, velocity);

        // Update rubber band visuals
        rubberBand.SetPosition(0, bandLeft.position);
        rubberBand.SetPosition(1, projectile.transform.position);
        rubberBand.SetPosition(2, bandRight.position);

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            MissionDemolition.SHOT_FIRED();

            // Disable rubber band after launch
            rubberBand.enabled = false;

            // Disable trajectory line after launch
            trajectoryLine.enabled = false;

            projectile = null;
        }
    }

    void DisplayTrajectory(Vector3 startPos, Vector3 velocity)
    {
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.enabled = true;

        Vector3[] points = new Vector3[trajectoryPoints];
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            // Physics formula: p = p0 + vt + 0.5gt²
            Vector3 point = startPos + velocity * t + 0.5f * Physics.gravity * t * t;
            points[i] = point;
        }

        trajectoryLine.SetPositions(points);
    }
}
