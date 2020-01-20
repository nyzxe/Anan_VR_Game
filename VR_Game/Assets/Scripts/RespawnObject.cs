using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour {
    [System.Serializable]
    private class Weapon {
        public GameObject weapon;
        public Vector3 spawnPosition;
    }

    [SerializeField]
    private Weapon[] weapons;

    // Start is called before the first frame update
    void Start() {
        SaveWeaponSpawns();
    }

    void SaveWeaponSpawns() {
        int i = 0;
        foreach (Weapon weapon in weapons) {
            weapons[i].spawnPosition = weapon.weapon.transform.position;
            i++;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Gun") {
            Debug.Log("respawning gun");
            int i = 0;
            foreach (Weapon weapon in weapons) {
                if (other.name == weapon.weapon.name) {
                    other.transform.position = weapon.spawnPosition;
                } else {
                    i++;
                }

            }
        }
    }


}
