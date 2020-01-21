using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Animator anim;
    public ScoreManager score;
    public bool locked;
    public bool isFriendly;
    public bool isDestroyed;
    bool scoreTallied;
    [SerializeField]
    float destroyTimer;
    Rigidbody rb;
    public int currentHealth;

    private void Start() {
        anim = transform.parent.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {

        if (currentHealth <= 0) {
            isDestroyed = true;
        }

        // If the target is hit and the score has not been adjusted, adjust the score accordingly.
        if (isDestroyed && !scoreTallied) {
            anim.Play("PopDown");
            if (isFriendly) {
                score.DeductScore();
                
            } else {
                score.AddScore();
            }
            scoreTallied = true;
        }

        // Sets the kinematic state based on the locked status.
        rb.isKinematic = locked;
    }

    private void OnTriggerEnter(Collider other) {
        // If a bomb or gun hits the target, enable isHit, unlock the target, and set the target to be destroyed.
        if (other.CompareTag("Bomb") || other.CompareTag("Gun")) {
            isDestroyed = true;
            locked = false;
            DestroyTarget();
        }
    }

    public void DestroyTarget() {
        Destroy(gameObject, destroyTimer);
    }

    




}
