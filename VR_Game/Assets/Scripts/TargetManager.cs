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

    [SerializeField]
    int row1MinTargets;
    [SerializeField]
    int row1MaxTargets;
    [SerializeField]
    int row2MinTargets;
    [SerializeField]
    int row2MaxTargets;
    [SerializeField]
    int row3MinTargets;
    [SerializeField]
    int row3MaxTargets;

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
    [SerializeField]
    float moveSpeed;
    float startTime;
    [SerializeField]
    float targetsDuration;
    float currentDuration;

    [SerializeField]
    bool moveTimeRecorded;
    [SerializeField]
    bool targetsPrepared;
    [SerializeField]
    bool targetsOnstage;
    [SerializeField]
    bool targetsDeployed;
    [SerializeField]
    bool targetsDown;

    int numberOfTargets1;
    int numberOfTargets2;
    int numberOfTargets3;
    [SerializeField]
    int numberOfRowsMoved;
    [SerializeField]
    int numberOfRows;

    AudioSource audio1;
    AudioSource audio2;
    AudioSource audio3;

    [SerializeField]
    AudioClip deployAudio;

    ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GetComponent<ScoreManager>();
        currentDuration = targetsDuration;
        currentRemoveDelay = removeDelay;
        currentDeployDelay = deployDelay;
        moveDistance = Vector3.Distance(offstageDestinations[0].transform.position, onstageDestinations[0].transform.position);
        audio1 = onstageDestinations[0].GetComponent<AudioSource>();
        audio2 = onstageDestinations[1].GetComponent<AudioSource>();
        audio3 = onstageDestinations[2].GetComponent<AudioSource>();
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
            if (targetsDeployed || moveTimeRecorded || targetsOnstage) {
                MoveOutTargets();
                if (!targetsOnstage) {
                    DestroyTargets();
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
        numberOfTargets1 = Random.Range(row1MinTargets, row1MaxTargets);

        // Once the player has finished three rows, add a second row.
        if (numberOfDestroyedRows >= 3) {
            numberOfRows = 2;
            numberOfTargets2 = Random.Range(row2MinTargets, row2MaxTargets);

            // Once the player has finished six rows, add a third row.
            if (numberOfDestroyedRows >= 6) {
                numberOfRows = 3;
                numberOfTargets3 = Random.Range(row3MinTargets, row3MaxTargets);
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

        if (numberOfTargets2 != 0) {
            // Instantiate the second row's targets with margin between them, and add them to the lists.
            for (int i = 0; i < numberOfTargets2; i++) {
                Vector3 calcMargin = offstageDestinations[1].position;
                calcMargin.z += (targetMargin * i);
                GameObject newTarget = Instantiate(targets[Random.Range(0, targets.Length)], calcMargin, Quaternion.Euler(0, -180, -90));
                allDeployedTargets.Add(newTarget);
                deployedTargets2.Add(newTarget);
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
            }
        }
    }

    void PrepareTemps() {

        // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
        temp1 = Instantiate(temp, offstageDestinations[0].transform.position, offstageDestinations[0].transform.rotation);

        // Reset temporary position of temp.
        tempPos = new Vector3();

        // Add all the Vector3 values of deployed targets in the row to the temporary position.
        for (int i = 0; i < deployedTargets1.Count; i++) {
            tempPos += deployedTargets1[i].transform.position;
        }

        // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
        // This results in the position at the center of the targets.
        temp1.transform.position = tempPos / deployedTargets1.Count;

        // Parent all the row's targets into temp.
        for (int i = 0; i < deployedTargets1.Count; i++) {
            deployedTargets1[i].transform.parent = temp1.transform;
        }

        // Prepares row 2 temps. This uses the same code as above.
        if (deployedTargets2.Count > 0) {

            // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
            temp2 = Instantiate(temp, offstageDestinations[1].transform.position, offstageDestinations[1].transform.rotation);

            // Reset temporary position of temp.
            tempPos = new Vector3();

            // Add all the Vector3 values of deployed targets in the row to the temporary position.
            for (int i = 0; i < deployedTargets2.Count; i++) {
                tempPos += deployedTargets2[i].transform.position;
            }

            // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
            // This results in the position at the center of the targets.
            temp2.transform.position = tempPos / deployedTargets2.Count;

            // Parent all the row's targets into temp.
            for (int i = 0; i < deployedTargets2.Count; i++) {
                deployedTargets2[i].transform.parent = temp2.transform;
            }

            // Moves row 3 targets. This uses the same code as above.
            if (deployedTargets3.Count > 0) {

                // Instantiate temporary object (temp). This will be used to move the targets onstage as a parent.
                temp3 = Instantiate(temp, offstageDestinations[2].transform.position, offstageDestinations[2].transform.rotation);

                // Reset temporary position of temp.
                tempPos = new Vector3();

                // Add all the Vector3 values of deployed targets in the row to the temporary position.
                for (int i = 0; i < deployedTargets3.Count; i++) {
                    tempPos += deployedTargets3[i].transform.position;
                }

                // Divide the temporary position's Vector3 value by the number of deployed targets in the row. 
                // This results in the position at the center of the targets.
                temp3.transform.position = tempPos / deployedTargets3.Count;

                // Parent all the row's targets into temp.
                for (int i = 0; i < deployedTargets3.Count; i++) {
                    deployedTargets3[i].transform.parent = temp3.transform;
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
            if (temp2 && onstageDestinations[1].transform.position.z - temp2.transform.position.z <= 0.01f) {
                numberOfRowsMoved = 2;
                if (temp3 && onstageDestinations[2].transform.position.z - temp3.transform.position.z <= 0.01f) {
                    numberOfRowsMoved = 3;
                }
            }
        }

        // If the temps have been moved, deploy them.
        if (numberOfRowsMoved == numberOfRows) {
            moveTimeRecorded = false;
            targetsOnstage = true;
        } 
    }



    // Move the targets offstage.
    void MoveOutTargets() {
        // Set isMoving to true so that the targets can't be shot.
        for (int i = 0; i < deployedTargets1.Count; i++) {
            if (deployedTargets1[i] != null) {
                deployedTargets1[i].GetComponentInChildren<Target>().isMoving = true;
            }
        }
        if (temp2) {
            for (int i = 0; i < deployedTargets2.Count; i++) {
                if (deployedTargets2[i] != null) {
                    deployedTargets2[i].GetComponentInChildren<Target>().isMoving = true;
                }
            }
            if (temp3) {
                for (int i = 0; i < deployedTargets3.Count; i++) {
                    if (deployedTargets3[i] != null) {
                        deployedTargets3[i].GetComponentInChildren<Target>().isMoving = true;
                    }
                }
            }
        }

        numberOfRowsMoved = 0;
        if (!moveTimeRecorded) {
            startTime = Time.time;
            moveTimeRecorded = true;
        }
        float distanceMoved = (Time.time - startTime) * moveSpeed;
        float fractionMoved = distanceMoved / moveDistance;
        
        if (temp1) {
            // Move temp to the designated onstage position for the row, thereby also moving the targets.
            temp1.transform.position = Vector3.Lerp(temp1.transform.position, offstageDestinations[0].transform.position, fractionMoved);
        }
        if (temp2) {
            // Move temp to the designated onstage position for the row, thereby also moving the targets.
            temp2.transform.position = Vector3.Lerp(temp2.transform.position, offstageDestinations[1].transform.position, fractionMoved);
            if (temp3) {
                // Move temp to the designated onstage position for the row, thereby also moving the targets.
                temp3.transform.position = Vector3.Lerp(temp3.transform.position, offstageDestinations[2].transform.position, fractionMoved);
            }
        }

        // Checks if each temp has been moved.
        if (temp1 && Mathf.Abs(offstageDestinations[0].transform.position.z - temp1.transform.position.z) <= 0.01f) {
            numberOfRowsMoved = 1;
            if (temp2 && Mathf.Abs(offstageDestinations[1].transform.position.z - temp2.transform.position.z) <= 0.01f) {
                numberOfRowsMoved = 2;
                if (temp3 && Mathf.Abs(offstageDestinations[2].transform.position.z - temp3.transform.position.z) <= 0.01f) {
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
        for (int i = 0; i < allDeployedTargets.Count; i++) {
            allDeployedTargets[i].GetComponent<Animator>().Play("PopUp");
            allDeployedTargets[i].GetComponentInChildren<Target>().isDeployed = true;
        }
        audio1.PlayOneShot(deployAudio);
        if (deployedTargets2.Count > 0) {
            audio2.PlayOneShot(deployAudio);
            if (deployedTargets3.Count > 0) {
                audio3.PlayOneShot(deployAudio);
            }
        }
        targetsDeployed = true;
    }

    // Destroy all targets if they are out of the stage.
    void DestroyTargets() {
        for (int i = 0; i < allDeployedTargets.Count; i++) {
            allDeployedTargets[i].transform.parent = null;
            Destroy(allDeployedTargets[i]);
        }

        // Destroy all temps.
        Destroy(temp1);
        if (temp2) {
            Destroy(temp2);
            if (temp3) {
                Destroy(temp3);
            }
        }
        ResetTargetState();
    }

    // Reset all target variables.
    void ResetTargetState() {
        targetsDown = false;
        targetsPrepared = false;
        targetsDeployed = false;
        currentDuration = targetsDuration;
        currentRemoveDelay = removeDelay;
        numberOfRowsMoved = 0;
        numberOfRows = 0;
    }

    // Check if the enemy targets are down.
    void CheckTargets() {
        for (int i = 0; i < allDeployedTargets.Count; i++) {
            Target t = allDeployedTargets[i].GetComponentInChildren<Target>();
            if (!t.isFriendly && !t.isDestroyed) {
                return;
            }
        }
        targetsDown = true;
    }
}
