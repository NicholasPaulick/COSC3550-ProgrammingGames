using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodySleep : MonoBehaviour
{
    private int sleepCountdown = 4;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (sleepCountdown > 0) {
            rb.Sleep();
            sleepCountdown--;
        }
    }
}
