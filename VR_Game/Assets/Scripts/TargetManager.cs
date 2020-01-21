using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] targets;

    List<GameObject> deployedTargets;
    int numberDeployed;
    [SerializeField]
    float targetDistance;

    public float deployDelay;
    public float popDelay;

    [SerializeField]
    float targetsDuration;
    float currentDuration;
    bool targetsPrepared;
    bool targetsMoved;
    bool targetsDeployed;
    bool targetsDown;

    ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GetComponent<ScoreManager>();
        currentDuration = targetsDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // Do things while the game is running.
        if (!scoreManager.gameEnded) {

            // Prepare targets if not prepared. ResetTargets() is used to trigger this.
            if (!targetsPrepared) {
                PrepareTargets();
            }
            
            // Destroy targets if all enemy targets in the batch is down.
            if (targetsDown) {
                MoveOutTargets();
                if (!targetsMoved) {
                    DestroyTargets();
                }
                
            // Destroy targets if the current time limit expires.
            } else if (targetsDeployed) {
                currentDuration -= Time.deltaTime;
                if (currentDuration <= 0f) {
                    MoveOutTargets();
                    if (!targetsMoved) {
                        DestroyTargets();
                    }
                }
            }

        // If there are leftover targets after the game has ended, remove them.
        } else if (targetsDeployed) {
            MoveOutTargets();
            if (!targetsMoved) {
                DestroyTargets();
            }
        }
    }

    // Instantiate targets in their positions offstage, and put them into lists.
    void PrepareTargets() {
        deployedTargets = new List<GameObject>();
        targetsPrepared = true;
    }

    // Move the prepared targets onstage.
    void MoveInTargets() {
        targetsMoved = true;
    }

    // Move the targets offstage.
    void MoveOutTargets() {
        targetsMoved = false;
    }

    // Pop up the targets.
    void DeployTargets() {
        foreach (GameObject target in deployedTargets) {
            target.GetComponent<Animator>().Play("PopUp");
            target.GetComponentInChildren<Target>().isDeployed = true;
        }
        targetsDeployed = true;
    }

    // Destroy all targets if they are out of the stage.
    void DestroyTargets() {
        if (!targetsMoved) {
            foreach (GameObject target in deployedTargets) {
                Destroy(target);
            }
            ResetTargetState();
        }
    }

    // Reset all target variables.
    void ResetTargetState() {
        targetsDown = false;
        targetsPrepared = false;
        targetsMoved = false;
        targetsDeployed = false;
        currentDuration = targetsDuration;
        numberDeployed = 0;
       
    }

    // Check if the enemy targets are down.
    void CheckTargets() {
        foreach (GameObject target in deployedTargets) {
            Target t = target.GetComponentInChildren<Target>();
            if (!t.isFriendly && !t.isDestroyed) {
                return;
            }
        }
        targetsDown = true;
    }
}
