using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour {
    [System.Serializable]
    private class Weapon {
        public GameObject weaponObject;
        [HideInInspector]
        public Vector3 spawnPosition;
    }

    [SerializeField]
    private Weapon[] weapons;
    [SerializeField]
    GameObject respawnEffect;

    // Start is called before the first frame update
    void Start() {
        SaveWeaponSpawns();
    }

    void SaveWeaponSpawns() {
        int i = 0;
        foreach (Weapon weapon in weapons) {
            weapons[i].spawnPosition = weapon.weaponObject.transform.position;
            i++;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Gun") {
            int i = 0;
            foreach (Weapon weapon in weapons) {
                if (other.name == weapon.weaponObject.name) {
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    other.transform.position = weapon.spawnPosition;
                    rb.isKinematic = false;
                    Destroy(Instantiate(respawnEffect, weapon.spawnPosition, Quaternion.identity), 2f);
                } else {
                    i++;
                }

            }
        }
        if (other.tag == "Bomb") {
            if (!other.GetComponent<Bomb>().fuseIsLit) {
                int i = 0;
                foreach (Weapon weapon in weapons) {
                    if (other.name == weapon.weaponObject.name) {
                        Rigidbody rb = other.GetComponent<Rigidbody>();
                        rb.isKinematic = true;
                        other.transform.position = weapon.spawnPosition;
                        rb.isKinematic = false;
                        Destroy(Instantiate(respawnEffect, weapon.spawnPosition, Quaternion.identity), 2f);
                    } else {
                        i++;
                    }

                }
            }
        }
    }


}
