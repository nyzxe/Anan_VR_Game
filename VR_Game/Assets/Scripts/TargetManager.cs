using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField]
    Target[] targets;

    List<GameObject> deployedTargets;

    public float deployDelay;
    public float popDelay;

    ScoreManager score;

    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ArrangeTargets() {

    }

    void DeployTargets() {

    }
}
