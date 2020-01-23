using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Animator anim;
    ScoreManager scoreManager;
    public bool locked;
    public bool isFriendly;
    public bool isDestroyed;
    public bool isDeployed;
    public bool isMoving;
    bool scoreTallied;
    [SerializeField]
    float destroyTimer;
    Rigidbody rb;
    public int currentHealth;

    [SerializeField]
    AudioClip downAudio;

    private void Awake() {
        isMoving = false;
        anim = transform.parent.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        isDeployed = false;
        scoreManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ScoreManager>();
    }

    private void Update() {

        if (!scoreManager.gameEnded) {

            if (currentHealth <= 0) {
                isDestroyed = true;
                isDeployed = false;
            }

            // If the target is hit and the score has not been adjusted, adjust the score accordingly.
            if (isDestroyed && !scoreTallied) {
                anim.Play("PopDown");
                GetComponent<AudioSource>().PlayOneShot(downAudio);
                if (isFriendly) {
                    scoreManager.DeductScore();

                } else {
                    scoreManager.AddScore();
                }
                scoreTallied = true;
            }

            // Sets the kinematic state based on the locked status.
            rb.isKinematic = locked;
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        // If a bomb or gun hits the target, enable isHit, unlock the target, and set the target to be destroyed.
        if (other.CompareTag("Bomb") || other.CompareTag("Gun")) {
            if (!isDestroyed && isDeployed) {
                isDestroyed = true;
                locked = false;
                DestroyTarget();
            }
            
        }
    }

    public void DestroyTarget() {
        Destroy(gameObject, destroyTimer);
    }

    




}
