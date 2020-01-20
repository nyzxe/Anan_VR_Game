using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    float explodeDelay =  3f;
    [SerializeField]
    float respawnDelay = 3f;
    [SerializeField]
    float radius = 5f;
    [SerializeField]
    float force = 500f;

    float currentRespawnTimer;
    float currentExplodeTimer;
    public bool fuseIsLit = false;
    bool hasExploded = false;

    Vector3 spawnPosition;

    public GameObject explosionEffect;
    Renderer renderer;
    GameObject colliders;
    GameObject crosshairs;

    OVRGrabbable grabber;
    public OVRInput.Button lightButton;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        crosshairs = GetComponent<Transform>().GetChild(0).gameObject;
        colliders = GetComponent<Transform>().GetChild(1).gameObject;
        grabber = GetComponent<OVRGrabbable>();

        // Save spawn position.
        spawnPosition = transform.position;
        // Initialize timers.
        currentExplodeTimer = explodeDelay;
        currentRespawnTimer = respawnDelay;
    }
    
    void Update()
    {
        // If the bomb is grabbed and the button is pressed, light the fuse.
        if (grabber.isGrabbed && OVRInput.GetDown(lightButton, grabber.grabbedBy.GetController()) && !hasExploded && !fuseIsLit) {
            fuseIsLit = true;
        }

        // If fuse is lit, count down the timer.
        if (fuseIsLit && !hasExploded) {
            currentExplodeTimer -= Time.deltaTime;
            // When the timer reaches 0, explode the bomb.
            if (currentExplodeTimer <= 0f) {
                Explode();
            }
        }

        // If bomb has exploded, count down the respawn delay.
        if (hasExploded) {
            currentRespawnTimer -= Time.deltaTime;
            // When the delay ends, reset the bomb.
            if (currentRespawnTimer <= 0f) {
                ResetBomb();
            }
        }
    }

    // Explode the bomb and disable the bomb.
    void Explode() {

        // Get all objects' colliders within blast radius..
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, radius);

        // If the involved objects have a rigidbody, apply explosive force to it.
        foreach (Collider nearbyObject in nearbyObjects) {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null) {
                Target target = nearbyObject.GetComponent<Target>();

                // If the involved object is a target, adjust the necessary variables for the target.
                if (target != null) {
                    target.locked = false;
                    target.isHit = true;
                    target.DestroyTarget();
                }
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        // Shake the camera.
        CameraShaker.Instance.ShakeOnce(3f, 8f, .1f, 1f);
        // Show explosion effect.
        //Instantiate(explosionEffect, transform.position, transform.rotation);
        // Disable the bomb's renderer.
        renderer.enabled = false;
        // Disable the bomb's crosshairs.
        crosshairs.SetActive(false);
        // Disable the bomb's colliders.
        colliders.SetActive(false);
        // Set exploded.
        hasExploded = true;
    }

    // Reset all bomb variables and respawn the bomb.
    void ResetBomb() {

        // Reset fuse state.
        fuseIsLit = false;
        // Reset exploded state.
        hasExploded = false;
        // Reset bomb timer.
        currentExplodeTimer = explodeDelay;
        // Reset respawn timer.
        currentRespawnTimer = respawnDelay;
        // Enable the bomb's renderer.
        renderer.enabled = true;
        // Enable the bomb's crosshairs.
        crosshairs.SetActive(true);
        // Enable the bomb's colliders.
        colliders.SetActive(true);
        // Teleport bomb to start position.
        transform.position = spawnPosition;
    }
}
