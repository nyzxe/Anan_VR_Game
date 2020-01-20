using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {
    [SerializeField]
    float destroyDelay;

    // Update is called once per frame
    void Update() {
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0f) {
            Destroy(gameObject);
        }

    }


}
