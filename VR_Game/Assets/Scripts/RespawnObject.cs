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

    [SerializeField]
    AudioClip respawnAudio;
    [SerializeField]
    AudioSource audioSource;

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
            RespawnGun(other);
        }
        if (other.tag == "Bomb") {
            if (!other.GetComponent<Bomb>().fuseIsLit) {
                RespawnBomb(other);
            }
        }
    }

    void RespawnGun(Collider other) {
        int i = 0;
        foreach (Weapon weapon in weapons) {
            if (other.name == weapon.weaponObject.name) {
                if (other.GetComponent<OVRGrabbable>().isGrabbed) {
                    return;
                }
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                other.transform.position = weapon.spawnPosition;
                rb.isKinematic = false;
                PlayRespawnAudio();
                Destroy(Instantiate(respawnEffect, weapon.spawnPosition, Quaternion.identity), 2f);
            } else {
                i++;
            }

        }
    }

    void RespawnBomb(Collider other) {
        int i = 0;
        foreach (Weapon weapon in weapons) {
            if (other.name == weapon.weaponObject.name) {
                if (other.GetComponent<OVRGrabbable>().isGrabbed) {
                    return;
                }
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                other.transform.position = weapon.spawnPosition;
                rb.isKinematic = false;
                PlayRespawnAudio();
                Destroy(Instantiate(respawnEffect, weapon.spawnPosition, Quaternion.identity), 2f);
            } else {
                i++;
            }

        }
    }

    public void PlayRespawnAudio() {
        if (respawnAudio != null && audioSource != null) {
            audioSource.PlayOneShot(respawnAudio);
        }
    }


}
