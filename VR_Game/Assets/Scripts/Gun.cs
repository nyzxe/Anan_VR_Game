using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform barrelLocation;
    OVRGrabbable grabber;
    public OVRInput.Button shootButton;
    [SerializeField]
    float shotPower;
    public enum GunType { Pistol, SMG, Shotgun, Rifle }
    public GunType gunType;
    [SerializeField]
    float fireRate;
    float lastFired;
    public int shotDamage;
    [SerializeField]
    int shotgunBulletCount;
    [SerializeField]
    float shotgunSpreadDeviation;

    void Start()
    {
        grabber = GetComponent<OVRGrabbable>();
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    void Update() {
        // If the held gun is a shotgun,
        if (gunType != GunType.SMG) {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun with spread.
            if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    for (int i = 0; i < shotgunBulletCount; i++) {
                        Shoot();
                    }
                }
            }

        // If the held gun is a submachine gun,
        } else if (gunType == GunType.SMG) {
            // While the gun is grabbed and the button is down, shoot the gun continuously.
            if (grabber.isGrabbed && OVRInput.Get(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    Shoot();
                }

            }
        // If the gun is a rifle or pistol,
        } else {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun.
            if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    Shoot();
                }
            }
        }
    }

    // Creates the bullet and propels it from the barrel.
    // Creates a raycast, applies a force if it hits a weapon, and knocks down any targets it hits.
    void Shoot()
    {
        lastFired = Time.time;
        if (gunType == GunType.Shotgun) {
            Vector3 direction = barrelLocation.transform.forward; // your initial aim.
            Vector3 spread = barrelLocation.transform.forward;
            spread += barrelLocation.transform.up * Random.Range(-shotgunSpreadDeviation, shotgunSpreadDeviation); // add random up or down (because random can get negative too)
            spread += barrelLocation.transform.right * Random.Range(-shotgunSpreadDeviation, shotgunSpreadDeviation); // add random left or right

            // Using random up and right values will lead to a square spray pattern. If we normalize this vector, we'll get the spread direction, but as a circle.
            // Since the radius is always 1 then (after normalization), we need another random call. 
            direction += spread.normalized * Random.Range(0f, 0.2f);
            Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(direction * shotPower);

        } else {
            Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        }
        
        
        //RaycastHit hit;
        //if (Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hit)) {
        //    //hit.gameObject.GetComponent<Rigidbody>()
        //    //if (hit.GetComponent<Rigidbody>())
        //    Debug.Log(hit.transform.name);
        //}
    }


}
