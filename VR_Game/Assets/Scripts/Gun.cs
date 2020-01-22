using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Gun : MonoBehaviour {

    float lastFired;

    [Tooltip("The button which will be used to shoot the gun.")]
    public OVRInput.Button shootButton;

    [SerializeField, Tooltip("The location of the tip of the barrel.")]
    Transform barrelLocation;
    [SerializeField, Tooltip("The prefab for the bullet impact visual effect.")]
    GameObject bulletImpact;

    OVRGrabbable grabbable;

    [Tooltip("The amount of damage one bullet will deal to a target on hit.")]
    public int shotDamage;
    [SerializeField, Tooltip("The amount of force the bullet will apply to a target.")]
    float shotPower;
    [SerializeField, Tooltip("The number of bullets that can be shot in one second.")]
    float fireRate;
    [SerializeField, Tooltip("The magnitude at which bullets will deviate from the center.")]
    float spreadDeviation;

    LineRenderer shotLine;
    List<LineRenderer> shotgunLines;

    [SerializeField, Tooltip("The duration of which bullets' tracers will appear for.")]
    float shotDuration;

    public enum GunType { Pistol, SMG, Shotgun, Rifle }
    public GunType gunType;

    [SerializeField, Tooltip("The prefab for the shotgun bullet tracer.")]
    GameObject shotgunLine;
    [SerializeField, Tooltip("The number of bullets which will be fired per shot.")]
    int shotgunBulletCount;

    [SerializeField]
    AudioClip gunshotAudio;
    

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
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
            if (grabbable.isGrabbed && OVRInput.GetDown(shootButton, grabbable.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    // Shake the camera.
                    CameraShaker.Instance.ShakeOnce(0.5f, 3f, 0.1f, 0.3f);
                    if (gunshotAudio != null) {
                        GetComponent<AudioSource>().PlayOneShot(gunshotAudio);
                    }
                    foreach (LineRenderer line in shotgunLines) {
                        Shoot(line);
                    }
                    
                }
            }
        }

        // If the held gun is a submachine gun,
        if (gunType == GunType.SMG) {
            // While the gun is grabbed and the button is down, shoot the gun continuously.
            if (grabbable.isGrabbed && OVRInput.Get(shootButton, grabbable.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    // Shake the camera.
                    CameraShaker.Instance.ShakeOnce(0.3f, 2f, 0.05f, 0.1f);
                    if (gunshotAudio != null) {
                        GetComponent<AudioSource>().PlayOneShot(gunshotAudio);
                    }
                    Shoot(shotLine);
                }

            }
        }

        // If the gun is a rifle,
        if (gunType == GunType.Rifle) {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun.
            if (grabbable.isGrabbed && OVRInput.GetDown(shootButton, grabbable.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    CameraShaker.Instance.ShakeOnce(0.5f, 3f, 0.1f, 0.3f);
                    if (gunshotAudio != null) {
                        GetComponent<AudioSource>().PlayOneShot(gunshotAudio);
                    }
                    Shoot(shotLine);
                }
            }
        }

        // If the gun is a pistol,
        if (gunType == GunType.Pistol) {
            // If the gun is grabbed, the button has been pressed once, and the gun is ready to fire, shoot the gun.
            if (grabbable.isGrabbed && OVRInput.GetDown(shootButton, grabbable.grabbedBy.GetController())) {
                if (Time.time - lastFired > 1 / fireRate) {
                    CameraShaker.Instance.ShakeOnce(0.5f, 2f, 0.1f, 0.3f);
                    if (gunshotAudio != null) {
                        GetComponent<AudioSource>().PlayOneShot(gunshotAudio);
                    }
                    Shoot(shotLine);
                }
            }
        }
    }

    // Creates a raycast with a visual effect and applies bullet effects to struck objects. 
    void Shoot(LineRenderer line) {
        
        //VibrationManager.singleton.TriggerVibration(gunshotAudio, grabbable.grabbedBy.GetController());
        VibrationManager.singleton.TriggerVibration(40, 2, 255, grabbable.grabbedBy.GetController());

        // Record the time of the last shot (this one is the new last shot).
        lastFired = Time.time;

        // Enable the bullet tracer.
        StartCoroutine(ShotEffect(line));

        // Set the start position of the bullet tracer at the barrel tip.
        line.SetPosition(0, barrelLocation.position);

        RaycastHit hit;
        // Start a raycast from the barrel tip to the calculated impact point.
        if (Physics.Raycast(barrelLocation.position, CalculateShotDeviation(), out hit)) {

            // Set the end position of the bullet tracer to the impact point.
            line.SetPosition(1, hit.point);

            // Instantiate the bullet impact effect, and destroy it 2 seconds later.
            Destroy(Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal)), 2f);

            // If the victim is a target, reduce the target's health.
            Target target = hit.transform.GetComponent<Target>();
            if (target != null && target.isDeployed) {
                target.currentHealth -= shotDamage;
            }

            // If the victim has a rigidbody, apply force to it.
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * shotPower);
            }
        }
    }

    // Calculates and returns a direction affected by shot deviation.
    Vector3 CalculateShotDeviation() {
        if (gunType != GunType.Rifle) {
            // Saves the initial aim direction. This will be used to return the result later.
            Vector3 direction = barrelLocation.transform.forward;

            // Saves the initial aim direction. This will be used as a temporary variable for calculations.
            Vector3 spread = barrelLocation.transform.forward;

            // Apply vertical deviation.
            spread += barrelLocation.transform.up * Random.Range(-spreadDeviation, spreadDeviation);

            // Apply horizontal deviation.
            spread += barrelLocation.transform.right * Random.Range(-spreadDeviation, spreadDeviation);

            // Apply the calculation to the return value, and round out the calculations to make it more organic.
            direction += spread.normalized * Random.Range(0f, 0.2f);

            return direction;
        } else {
            return barrelLocation.transform.forward;
        }
    }

    // Toggles the LineRenderer for a duration.
    IEnumerator ShotEffect(LineRenderer line) {
        line.enabled = true;
        yield return shotDuration;
        line.enabled = false;
    }


}
