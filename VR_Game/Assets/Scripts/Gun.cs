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
    float shotPower = 1500f;

    void Start()
    {
        grabber = GetComponent<OVRGrabbable>();
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    void Update() {
        // If the gun is grabbed and the button is pressed, shoot the gun.
        if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
            Shoot();
        }
    }

    // Creates the bullet and propels it from the barrel.
    // Creates a raycast, applies a force if it hits a weapon, and knocks down any targets it hits.
    void Shoot()
    {
        //Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hit)) {
            //hit.gameObject.GetComponent<Rigidbody>()
            //if (hit.GetComponent<Rigidbody>())
            Debug.Log(hit.transform.name);
        }
    }


}
