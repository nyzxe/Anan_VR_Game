using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] targets;

    List<GameObject> deployedTargets;

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
        if (!scoreManager.gameEnded) {
            PrepareTargets();
            if (targetsDown) {
                ResetTargets();
                PrepareTargets();
            } else if (targetsDeployed) {
                currentDuration -= Time.deltaTime;
                if (currentDuration <= 0f) {
                    RemoveTargets();
                    ResetTargets();
                }
            }
        }
    }

    void PrepareTargets() {
        targetsPrepared = true;
    }

    void MoveTargets() {
        targetsMoved = true;
    }

    void DeployTargets() {
        targetsDeployed = true;
    }

    void RemoveTargets() {

    }

    void ResetTargets() {
        targetsDown = false;
        targetsPrepared = false;
        targetsMoved = false;
        targetsDeployed = false;
        currentDuration = targetsDuration;
    }

    void CheckTargets() {

    }
}
