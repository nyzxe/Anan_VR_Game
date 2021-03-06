﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using OculusSampleFramework;

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
    [SerializeField]
    GameObject fuseTip;
    [SerializeField]
    GameObject fuseEffect;
    GameObject currentFuseEffect;
    public bool fuseIsLit = false;
    bool hasExploded = false;

    Vector3 spawnPosition;

    public GameObject explosionEffect;
    Renderer rend;
    GameObject colliders;
    GameObject crosshairs;
    Rigidbody rb;

    OVRGrabbable grabbable;
    public OVRInput.Button lightButton;

    [SerializeField]
    AudioClip explosionAudio;
    [SerializeField]
    AudioClip fuseAudio;
    [SerializeField]
    RespawnObject respawn;

    void Start()
    {
        rend = GetComponent<Renderer>();
        crosshairs = GetComponent<Transform>().GetChild(0).gameObject;
        colliders = GetComponent<Transform>().GetChild(1).gameObject;
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();

        // Save spawn position.
        spawnPosition = transform.position;
        // Initialize timers.
        currentExplodeTimer = explodeDelay;
        currentRespawnTimer = respawnDelay;
    }
    
    void Update()
    {
        // If the bomb is grabbed and the button is pressed, light the fuse.
        if (grabbable.isGrabbed && OVRInput.GetDown(lightButton, grabbable.grabbedBy.GetController()) && !hasExploded && !fuseIsLit) {
            fuseIsLit = true;
            if (fuseAudio != null) {
                GetComponent<AudioSource>().PlayOneShot(fuseAudio);
            }
            currentFuseEffect = Instantiate(fuseEffect, fuseTip.transform.position, fuseTip.transform.rotation);
            currentFuseEffect.transform.parent = gameObject.transform;
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
            Destroy(currentFuseEffect);
            currentRespawnTimer -= Time.deltaTime;
            // When the delay ends, reset the bomb.
            if (currentRespawnTimer <= 0f) {
                ResetBomb();
            }
        }
    }

    // Explode the bomb and disable the bomb.
    void Explode() {
        // Stop playing the fuse audio clip.
        GetComponent<AudioSource>().Stop();
        
        // If the bomb is still grabbed, ungrab it.
        if (grabbable.isGrabbed) {
            grabbable.grabbedBy.ForceRelease(grabbable);
        }

        // Get all objects' colliders within blast radius..
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, radius);

        // If the involved objects have a rigidbody, apply explosive force to it.
        for (int i = 0; i < nearbyObjects.Length; i++) {
            Rigidbody rb = nearbyObjects[i].GetComponent<Rigidbody>();

            if (rb != null) {
                Target target = nearbyObjects[i].GetComponent<Target>();

                // If the involved object is a target, adjust the necessary variables for the target.
                if (target != null && target.isDeployed) {
                    target.locked = false;
                    target.isDestroyed = true;
                }
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        // Shake the camera.
        CameraShaker.Instance.ShakeOnce(3f, 6f, .1f, 1f);
        // Play explosion audio.
        if (explosionAudio != null) {
            GetComponent<AudioSource>().PlayOneShot(explosionAudio);
        }
        // Show explosion effect.
        Instantiate(explosionEffect, transform.position, transform.rotation);
        // Disable the bomb's renderer.
        rend.enabled = false;
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
        // Reset bomb timer.
        currentExplodeTimer = explodeDelay;

        if (!grabbable.isGrabbed) {
            // Reset exploded state.
            hasExploded = false;
            // Reset respawn timer.
            currentRespawnTimer = respawnDelay;
            // Enable the bomb's renderer.
            rend.enabled = true;
            // Enable the bomb's crosshairs.
            crosshairs.SetActive(true);
            // Enable the bomb's colliders.
            colliders.SetActive(true);
            // Temporarily set object to isKinematic.
            rb.isKinematic = true;
            // Teleport bomb to start position.
            transform.position = spawnPosition;
            // Reset isKinematic.
            rb.isKinematic = false;
            // Play respawn audio.
            respawn.PlayRespawnAudio();
        }
        
    }
}
