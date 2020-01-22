using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{

    [SerializeField]
    GameObject[] targets;

    [SerializeField]
    Transform[] offstageDestinations;
    [SerializeField]
    Transform[] onstageDestinations;
    [SerializeField]
    GameObject temp;
    GameObject temp1;
    GameObject temp2;
    GameObject temp3;

    List<GameObject> allDeployedTargets;
    List<GameObject> deployedTargets1;
    List<GameObject> deployedTargets2;
    List<GameObject> deployedTargets3;

    int numberDeployed;
    [SerializeField]
    float targetMargin;
    [SerializeField]
    int numberOfDestroyedRows = 0;
    public float deployDelay;
    [SerializeField]
    float currentDeployDelay;
    [SerializeField]
    float removeDelay;
    float currentRemoveDelay;
    Vector3 tempPos;

    float moveDistance;
    float moveSpeed = 0.5f;
    [SerializeField]
    bool moveTimeRecorded;
    float startTime;

    [SerializeField]
    float targetsDuration;
    float currentDuration;

    [SerializeField]
    bool targetsPrepared;
    [SerializeField]
    bool targetsOnstage;
    [SerializeField]
    bool targetsDeployed;
    [SerializeField]
    bool targetsDown;
    [SerializeField]
    bool targetsDestroyed;

    int numberOfTargets1;
    int numberOfTargets2;
    int numberOfTargets3;
    [SerializeField]
    int numberOfRowsMoved;
    [SerializeField]
    int numberOfRows;

    ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GetComponent<ScoreManager>();
        currentDuration = targetsDuration;
        currentRemoveDelay = removeDelay;
        currentDeployDelay = deployDelay;
        moveDistance = Vector3.Distance(offstageDestinations[0].transform.position, onstageDestinations[0].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Do things while the game is running.
        if (!scoreManager.gameEnded) {
            
            // Prepare targets if not prepared. ResetTargets() is used to trigger this.
            if (!targetsPrepared && !targetsOnstage) {
                PrepareTargets();
            } else if (!targetsOnstage) {
                MoveInTargets();
            }

            if (targetsOnstage && !targetsDeployed && !targetsDown) {
                currentDeployDelay -= Time.deltaTime;
                if (currentDeployDelay <= 0f) {
                    DeployTargets();
                }
                    
            }

            if (targetsOnstage && targetsDeployed && !targetsDown) {
                CheckTargets();
            }
            
            // Destroy targets if all enemy targets in the batch is down.
            if (targetsDown) {
                currentRemoveDelay -= Time.deltaTime;
                if (currentRemoveDelay <= 0f) {
                    MoveOutTargets();
                    if (!targetsOnstage) {
                        DestroyTargets();
                        numberOfDestroyedRows++;
                    }
                

                }
                
            // Destroy targets if the current time limit expires.
            } else if (targetsDeployed) {
                currentDuration -= Time.deltaTime;
                if (currentDuration <= 0f) {
                    MoveOutTargets();
                    if (!targetsOnstage) {
                        DestroyTargets();
                    }
                }
            }

        // If there are leftover targets after the game has ended, remove them.
        } else {
            if (targetsDeployed || moveTimeRecorded) {
                MoveOutTargets();
                if (!targetsOnstage) {
                    DestroyTargets();
                    ResetTargetState();
                    numberOfDestroyedRows = 0;
                    numberOfTargets1 = 0;
                    numberOfTargets2 = 0;
                    numberOfTargets3 = 0;
                }
            }
            
        }
    }

    
    void DetermineNumberOfTargets() {
        allDeployedTargets = new List<GameObject>();
        deployedTargets1 = new List<GameObject>();
        deployedTargets2 = new List<GameObject>();
        deployedTargets3 = new List<GameObject>();

        numberOfRows = 1;
        numberOfTargets1 = Random.Range(2, 4);

        // Once the player has finished three rows, add a second row.
        if (numberOfDestroyedRows >= 3) {
            numberOfRows = 2;
            numberOfTargets2 = Random.Range(3, 5);

            // Once the player has finished six rows, add a third row.
            if (numberOfDestroyedRows >= 6) {
                numberOfRows = 3;
                numberOfTargets3 = Random.Range(2, 4);
            } else {
                numberOfTargets3 = 0;
            }
        } else {
            numberOfTargets2 = 0;
            numberOfTargets3 = 0;
        }
    }

    void InstantiateTargets() {
        // Instantiate the first row's targets with margin between them, and add them to the lists.
        for (int i = 0; i < numberOfTargets1; i++) {
            Vector3 calcMargin = offstageDestinations[0].position;
            calcMargin.z += (targetMargin * i);
            GameObject newTarget = Instantiate(targets[Random.Range(0, targets.Length)], calcMargin, Quaternion.Euler(0, -180, -90));
            allDeployedTargets.Add(newTarget);
            deployedTargets1.Add(newTarget);
        }

        // Ensure that there is at least one enemy target in the row.
        bool row1Prepared = false;
        foreach (GameObject target in deployedTargets1) {
            if (!target.GetComponentInChildren<Target>().isFriendly) {
                row1Prepared = true;
            }
        }
        if (!row1Prepared) {
            deployedTargets1.RemoveAt(0);
            deployedTargets1.Add(targets[2]);
        }

        if (numberOfTargets2 != 0) {
            // Instantiate the second row's targets with margin between them, and add them to the lists.
            for (int i = 0; i < numberOfTargets2; i++) {
                Vector3 calcMargin = offstageDestinations[1].position;
                calcMargin.z += (targetMargin * i);
                GameObject newTarget = Instantiate(targets[Random.Range(0, targets.Length)], calcMargin, Quaternion.Euler(0, -180, -90));
                allDeployedTargets.Add(newTarget);
                deployedTargets2.Add(newTarget);
            }

            // Ensure that there is at least one enemy target in the row.
            bool row2Prepared = false;
            foreach (GameObject target in deployedTargets2) {
                if (!target.GetComponentInChildren<Target>().isFriendly) {
                    row2Prepared = true;
                }
            }
            if (!row2Prepared) {
                deployedTargets2.RemoveAt(0);
                deployedTargets2.Add(targets[2]);

            }
            if (numberOfTargets3 != 0) {
                // Instantiate the third row's targets with margin between them, and add them to the lists.
                for (int i = 0; i < numberOfTargets3; i++) {
                    Vector3 calcMargin = offstageDestinations[2].position;
                    calcMargin.z += (targetMargin * i);
                    GameObject newTarget = Instantiate(targets[Random.Range(0, targets.Length)], calcMargin, Quaternion.Euler(0, -180, -90));
                    allDeployedTargets.Add(newTarget);
                    deployedTargets3.Add(newTarget);
                }

                // Ensure that there is at least one enemy target in the row.
                bool row3Prepared = false;
                foreach (GameObject target in deployedTargets3) {
                    if (!target.GetComponentInChildren<Target>().isFriendly) {
                        row3Prepared = true;
                    }
                }
                if (!row3Prepared) {
                    deployedTargets3.RemoveAt(0);
                    deployedTargets3.Add(targets[2]);
                }
            }
        }
    }

    void PrepareTemps() {
        // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
        temp1 = Instantiate(temp, offstageDestinations[0].transform.position, offstageDestinations[0].transform.rotation);

        // Reset temporary position of temp.
        tempPos = new Vector3();

        // Add all the Vector3 values of deployed targets in the row to the temporary position.
        foreach (GameObject target in deployedTargets1) {
            tempPos += target.transform.position;
        }

        // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
        // This results in the position at the center of the targets.
        temp1.transform.position = tempPos / deployedTargets1.Count;

        // Parent all the row's targets into temp.
        foreach (GameObject target in deployedTargets1) {
            target.transform.parent = temp1.transform;
        }

        // Prepares row 2 temps. This uses the same code as above.
        if (deployedTargets2.Count > 0) {

            // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
            temp2 = Instantiate(temp, offstageDestinations[1].transform.position, offstageDestinations[1].transform.rotation);

            // Reset temporary position of temp.
            tempPos = new Vector3();

            // Add all the Vector3 values of deployed targets in the row to the temporary position.
            foreach (GameObject target in deployedTargets2) {
                tempPos += target.transform.position;
            }

            // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
            // This results in the position at the center of the targets.
            temp2.transform.position = tempPos / deployedTargets2.Count;

            // Parent all the row's targets into temp.
            foreach (GameObject target in deployedTargets2) {
                target.transform.parent = temp2.transform;
            }

            // Moves row 3 targets. This uses the same code as above.
            if (deployedTargets3.Count > 0) {

                // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
                temp3 = Instantiate(temp, offstageDestinations[2].transform.position, offstageDestinations[2].transform.rotation);

                // Reset temporary position of temp.
                tempPos = new Vector3();

                // Add all the Vector3 values of deployed targets in the row to the temporary position.
                foreach (GameObject target in deployedTargets3) {
                    tempPos += target.transform.position;
                }

                // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
                // This results in the position at the center of the targets.
                temp3.transform.position = tempPos / deployedTargets3.Count;

                // Parent all the row's targets into temp.
                foreach (GameObject target in deployedTargets3) {
                    target.transform.parent = temp3.transform;
                }
            }
        }
    }

    // Instantiate targets in their positions offstage, and put them into lists.
    void PrepareTargets() {
        DetermineNumberOfTargets();
        InstantiateTargets();
        PrepareTemps();
        targetsPrepared = true;
    }

    // Move the prepared targets onstage.
    void MoveInTargets() {
        if (!moveTimeRecorded) {
            startTime = Time.time;
            moveTimeRecorded = true;
        }
        float distanceMoved = (Time.time - startTime) * moveSpeed;
        float fractionMoved = distanceMoved / moveDistance;
        // Move temp to the designated onstage position for the row, thereby also moving the targets.
        temp1.transform.position = Vector3.Lerp(temp1.transform.position, onstageDestinations[0].transform.position, fractionMoved);
        if (temp2) {
            // Move temp to the designated onstage position for the row, thereby also moving the targets.
            temp2.transform.position = Vector3.Lerp(temp2.transform.position, onstageDestinations[1].transform.position, fractionMoved);
            if (temp3) {
                // Move temp to the designated onstage position for the row, thereby also moving the targets.
                temp3.transform.position = Vector3.Lerp(temp3.transform.position, onstageDestinations[2].transform.position, fractionMoved);
            }
        }

        // Checks if each temp has been moved.
        if (onstageDestinations[0].transform.position.z - temp1.transform.position.z <= 0.01f) {
            numberOfRowsMoved = 1;
            if (temp2 && onstageDestinations[1].transform.position.z - temp2.transform.position.z <= 1f) {
                numberOfRowsMoved = 2;
                if (temp3 && onstageDestinations[2].transform.position.z - temp3.transform.position.z <= 1f) {
                    numberOfRowsMoved = 3;
                }
            }
        }

        // If the temps have been moved, deploy them.
        if (numberOfRowsMoved == numberOfRows) {
            moveTimeRecorded = false;
            targetsOnstage = true;
            numberOfRowsMoved = 0;
            
        } 
    }



    // Move the targets offstage.
    void MoveOutTargets() {
        if (!moveTimeRecorded) {
            startTime = Time.time;
            moveTimeRecorded = true;
        }
        float distanceMoved = (Time.time - startTime) * moveSpeed;
        float fractionMoved = distanceMoved / moveDistance;
        targetsOnstage = true;
        // Move temp to the designated offstage position for the row, thereby also moving the targets.
        temp1.transform.position = Vector3.Lerp(temp1.transform.position, offstageDestinations[0].transform.position, fractionMoved);
        if (temp2) {
            // Move temp to the designated offstage position for the row, thereby also moving the targets.
            temp2.transform.position = Vector3.Lerp(temp2.transform.position, offstageDestinations[1].transform.position, fractionMoved);
            if (temp3) {
                // Move temp to the designated offstage position for the row, thereby also moving the targets.
                temp3.transform.position = Vector3.Lerp(temp3.transform.position, offstageDestinations[2].transform.position, fractionMoved);
            }
        }

        // Checks if each temp has been moved.
        if (offstageDestinations[0].transform.position.z - temp1.transform.position.z <= 0.01f) {
            numberOfRowsMoved = 1;
            if (temp2 && offstageDestinations[1].transform.position.z - temp2.transform.position.z <= 1f) {
                numberOfRowsMoved = 2;
                if (temp3 && offstageDestinations[2].transform.position.z - temp3.transform.position.z <= 1f) {
                    numberOfRowsMoved = 3;
                }
            }
        }

        // If the temps have been moved, destroy them.
        if (numberOfRowsMoved == numberOfRows) {
            moveTimeRecorded = false;
            targetsOnstage = false;
            
        }
        

    }

    // Pop up the targets.
    void DeployTargets() {
        currentDeployDelay = deployDelay;
        foreach (GameObject target in allDeployedTargets) {
            target.GetComponent<Animator>().Play("PopUp");
            target.GetComponentInChildren<Target>().isDeployed = true;
        }
        targetsDeployed = true;
    }

    // Destroy all targets if they are out of the stage.
    void DestroyTargets() {
        foreach (GameObject target in allDeployedTargets) {
            target.transform.parent = null;
            Destroy(target);
        }

        // Destroy all temps.
        Destroy(temp1);
        if (temp2) {
            Destroy(temp2);
            if (temp3) {
                Destroy(temp3);
            }
        }
        targetsDestroyed = true;
        ResetTargetState();
    }

    // Reset all target variables.
    void ResetTargetState() {
        targetsDown = false;
        targetsPrepared = false;
        targetsDeployed = false;
        targetsDestroyed = false;
        currentDuration = targetsDuration;
        currentRemoveDelay = removeDelay;
        numberOfRowsMoved = 0;
        numberOfRows = 0;
        numberDeployed = 0;
       
    }

    // Check if the enemy targets are down.
    void CheckTargets() {
        foreach (GameObject target in allDeployedTargets) {
            Target t = target.GetComponentInChildren<Target>();
            if (!t.isFriendly && !t.isDestroyed) {
                return;
            }
        }
        targetsDown = true;
    }
}
