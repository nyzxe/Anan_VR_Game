using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform barrelLocation;
    OVRGrabbable grabber;
    public OVRInput.Button shootButton;

    public float shotPower = 1500f;

    void Start()
    {
        grabber = GetComponent<OVRGrabbable>();
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    void Update() {
        if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
            Shoot();
        }
    }

    void Shoot()
    {
       Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
    }


}
