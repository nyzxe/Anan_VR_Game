using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Animator anim;
    public ScoreManager score;
    public bool locked;
    public bool isFriendly;
    public bool isHit;
    [SerializeField]
    float destroyTimer;
    bool scoreTallied;
    Rigidbody rb;

    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        // Sets the kinematic state based on the locked status.
        rb.isKinematic = locked;

        // If the target is hit and the score has not been adjusted, adjust the score accordingly.
        if (isHit && !scoreTallied) {
            if (isFriendly) {
                score.DeductScore();
                
            } else {
                score.AddScore();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // If a bullet hits the target, enable isHit and move down the target.
        //if (other.CompareTag("Bullet")) {
        //    isHit = true;
        //    anim.SetBool("isDown", true);

        // If a bomb or gun hits the target, enable isHit, unlock the target, and set the target to be destroyed.
        if (other.CompareTag("Bomb") || other.CompareTag("Gun")) {
            isHit = true;
            locked = false;
            DestroyTarget();
        }
    }

    public void DestroyTarget() {
        Destroy(gameObject, destroyTimer);
    }

    




}
