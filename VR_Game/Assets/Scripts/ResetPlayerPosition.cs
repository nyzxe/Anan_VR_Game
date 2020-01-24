using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    [SerializeField]
    OVRInput.Button resetButton;
    [SerializeField]
    Transform spawnPosition;
    [SerializeField]
    GameObject player;

    private void Start() {
        player.transform.position = spawnPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(resetButton)) {
            player.transform.position = spawnPosition.position;
        }
    }
}
