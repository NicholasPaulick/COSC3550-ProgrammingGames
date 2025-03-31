using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S;
    static public GameObject POI; // The point of interest

    public enum eView {
        none,
        slingshot,
        castle,
        both
    };

    [Header("Inscribed")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject viewBothGO;

    [Header("Dynamic")]
    public float camZ; // The desired z position of the camera
    public eView nextView = eView.slingshot;

    private void Awake() {
        S = this;
        camZ = this.transform.position.z;
    }

    private void FixedUpdate() {
        Vector3 destination = Vector3.zero;

        if (POI != null) {
            // If the POI has a rigidbody, check to see if it is sleeping
            Rigidbody poiRigidbody = POI.GetComponent<Rigidbody>();
            if ((poiRigidbody != null) && poiRigidbody.IsSleeping()) {
                POI = null;
            }
        }

        if (POI != null) {
            destination = POI.transform.position;
        }

        // Limit the minimum values of destination.x and destination.y
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        // Interpolate frorm the current Camera position to the destination
        destination = Vector3.Lerp(this.transform.position, destination, easing);
        // Force destination.z to be camZ to keep the camera at the same distance
        destination.z = camZ;
        // Set the camera to the destination
        this.transform.position = destination;
        // Set the orthographicSize of the Camera to keep Ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }

    public void SwitchView(eView newView) {
        if (newView == eView.none) {
            newView = nextView;
        }
        switch(newView) {
            case eView.slingshot:
                POI = null;
                newView = eView.castle;
                break;
            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();
                newView = eView.both;
                break;
            case eView.both:
                POI = viewBothGO;
                newView = eView.slingshot;
                break;
        }
    }

    public void SwitchView() {
        SwitchView(eView.none);
    }

    static public void SWITCH_VIEW(eView newView) {
        S.SwitchView(newView);
    }
}
