using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Animator anim;
    public ScoreManager scoreManager;
    TargetManager targetManager;
    public bool locked;
    public bool isFriendly;
    public bool isDestroyed;
    public bool isDeployed;
    bool isUp;
    bool scoreTallied;
    [SerializeField]
    float destroyTimer;
    Rigidbody rb;
    public int currentHealth;

    float popupDelay = 2f;
    public float currentDelay;

    private void Start() {
        anim = transform.parent.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentDelay = popupDelay;
        isUp = false;
    }

    private void Update() {

        if (!scoreManager.gameEnded) {
            if (!isUp) {

                currentDelay -= Time.deltaTime;
                if (currentDelay <= 0f) {
                    anim.Play("PopUp");
                    isUp = true;
                }
            }

            if (currentHealth <= 0) {
                isDestroyed = true;
            }

            // If the target is hit and the score has not been adjusted, adjust the score accordingly.
            if (isDestroyed && !scoreTallied) {
                anim.Play("PopDown");
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
            if (!isDestroyed) {
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
