using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    const int LOOKBACK_COUNT = 10;
    static List<Projectile> PROJECTILES = new List<Projectile>();

    [SerializeField]
    private bool _awake = true;
    public bool awake {
        get {return _awake;}
        private set {_awake = value;}
    }

    private Vector3 prevPos;
    // This private List storre the history of Projectile's move distance
    private List<float> deltas = new List<float>();
    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        awake = true;
        prevPos = new Vector3(1000, 1000, 0);
        deltas.Add(1000);

        PROJECTILES.Add(this);
    }

    private void FixedUpdate() {
        if (rb.isKinematic || !awake) return;

        Vector3 deltaV3 = transform.position - prevPos;
        deltas.Add(deltaV3.magnitude);
        prevPos = transform.position;

        // Limit lookback
        while (deltas.Count > LOOKBACK_COUNT) {
            deltas.RemoveAt(0);
        }

        // Iterate over deltas and find the greatest
        float maxDelta = 0;
        foreach (float f in deltas) {
            if (f > maxDelta) maxDelta = f;
        }

        // If the Projecile hasn't move more than the sleepThreshold
        if (maxDelta <= Physics.sleepThreshold) {
            awake = false;
            rb.Sleep();
        }
    }

    private void OnDestroy() {
        PROJECTILES.Remove(this);
    }

    static public void DESTROY_PROJECTILES() {
        foreach (Projectile p in PROJECTILES) {
            Destroy(p.gameObject);
        }
    }
}
