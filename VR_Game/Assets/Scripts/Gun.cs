using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    Transform barrelLocation;
    [SerializeField]
    GameObject bulletImpact;
    OVRGrabbable grabber;
    public OVRInput.Button shootButton;
    [SerializeField]
    float shotPower;
    LineRenderer shotLine;
    [SerializeField]
    GameObject shotgunLine;
    List<LineRenderer> shotgunLines;
    [SerializeField]
    float shotDuration;
    public enum GunType { Pistol, SMG, Shotgun, Rifle }
    public GunType gunType;
    [SerializeField]
    float fireRate;
    float lastFired;
    public int shotDamage;
    [SerializeField]
    float spreadDeviation;
    [SerializeField]
    int shotgunBulletCount;
    

    void Start()
    {
        grabber = GetComponent<OVRGrabbable>();
        if (barrelLocation == null)
            barrelLocation = transform;
        

        if (gunType == GunType.Shotgun) {
            shotgunLines = new List<LineRenderer>();
            // Instantiate required LineRenderer objects for the shotgun and add them to a list.
            for (int i = 0; i < shotgunBulletCount; i++) {
                GameObject tempLine = Instantiate(shotgunLine, gameObject.transform);
                shotgunLines.Add(tempLine.GetComponent<LineRenderer>());
            }

        } else {
            shotLine = GetComponent<LineRenderer>();
        }
        
    }

    void Update() {
        // If the held gun is a shotgun,
        if (gunType == GunType.Shotgun) {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun with spread.
            if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    foreach (LineRenderer line in shotgunLines) {
                        Shoot(line);
                    }
                    
                }
            }
        }

        // If the held gun is a submachine gun,
        if (gunType == GunType.SMG) {
            // While the gun is grabbed and the button is down, shoot the gun continuously.
            if (grabber.isGrabbed && OVRInput.Get(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    Shoot();
                }

            }
        }

        // If the gun is a rifle or pistol,
        if (gunType == GunType.Pistol || gunType == GunType.Rifle) {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun.
            if (grabber.isGrabbed && OVRInput.GetDown(shootButton, grabber.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    Shoot();
                }
            }
        }
    }

    // Creates a raycast, applies a force if it hits a weapon, and knocks down any targets it hits.
    void Shoot()
    {
        // Record the time of the last shot (this one is the new last shot).
        lastFired = Time.time;

        StartCoroutine(ShotEffect(shotLine));

        shotLine.SetPosition(0, barrelLocation.position);

        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, CalculateShotDeviation(), out hit)) {
            shotLine.SetPosition(1, hit.point);
            Destroy(Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal)), 2f);
            
            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.currentHealth -= shotDamage;
            }
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * shotPower);
            }
        }
    }

    // Creates a raycast, applies a force if it hits a weapon, and knocks down any targets it hits. 
    // This is the shotgun version which makes use of the instantiated list of lines.
    void Shoot(LineRenderer shotgunLine) {
        // Record the time of the last shot (this one is the new last shot).
        lastFired = Time.time;

        StartCoroutine(ShotEffect(shotgunLine));

        shotgunLine.SetPosition(0, barrelLocation.position);

        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, CalculateShotDeviation(), out hit)) {
            shotgunLine.SetPosition(1, hit.point);
            Destroy(Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal)), 2f);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.currentHealth -= shotDamage;
            }
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * shotPower);
            }
        }
    }
    // Calculates and returns a direction affected by shot deviation.
    Vector3 CalculateShotDeviation() {
        Vector3 direction = barrelLocation.transform.forward; // your initial aim.
        Vector3 spread = barrelLocation.transform.forward;
        spread += barrelLocation.transform.up * Random.Range(-spreadDeviation, spreadDeviation); // add random up or down (because random can get negative too)
        spread += barrelLocation.transform.right * Random.Range(-spreadDeviation, spreadDeviation); // add random left or right
        direction += spread.normalized * Random.Range(0f, 0.2f);
        return direction;
    }

    // Toggles the LineRenderer for a duration.
    IEnumerator ShotEffect(LineRenderer line) {
        line.enabled = true;
        yield return shotDuration;
        line.enabled = false;
    }


}
