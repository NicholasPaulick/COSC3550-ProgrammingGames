using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    // A staic field accessible by code anywhere
    static public bool goalMet = false;

    private void OnTriggerEnter(Collider other) {
        // When the trigger is hit by something
        // Check if it is a projectile
        Projectile proj = other.gameObject.GetComponent<Projectile>();
        if (proj != null) {
            // If so, set goalMet to true
            goalMet = true;
            // Also set the alpha of the color to a higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color col = mat.color;
            col.a = 0.75f;
            mat.color = col;
        }
    }
}
