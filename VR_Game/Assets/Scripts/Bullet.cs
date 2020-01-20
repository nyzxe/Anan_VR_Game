using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float destroyDelay;
    bool toDestroy;
    [SerializeField]
    ConstantForce customGravity;

    // Update is called once per frame
    void Update()
    {
        if (toDestroy) {
            destroyDelay -= Time.deltaTime;
            if (destroyDelay <= 0f) {
                Destroy(gameObject);
            }
        }

    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Target")) {
            Destroy(gameObject);
        }
        toDestroy = true;
        
    }

    
}
