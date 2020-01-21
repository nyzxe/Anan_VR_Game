using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    int targetScore;
    public int currentScore;
    [SerializeField]
    Text scoreText;
    [HideInInspector]
    public bool gameEnded;
    public bool resultGiven;
    public float timeLimit;
    float currentTimer;
    public Light resultLight;
    Color defaultLightColor;

    [SerializeField]
    float resultDelay;
    float currentDelay;

    [SerializeField]
    Color winColor;
    [SerializeField]
    Color loseColor;

    // Start is called before the first frame update
    void Start()
    {
        defaultLightColor = resultLight.color;
        currentTimer = timeLimit;
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is ongoing, count down the timer.
        if (!gameEnded) {
            currentTimer -= Time.deltaTime;
            scoreText.text = currentScore.ToString();
            if (currentTimer <= 0f) {
                EndGame();
            }

        // If the game has ended, count down the result delay.
        } else if (gameEnded && !resultGiven) {
            currentDelay -= Time.deltaTime;
            if (currentDelay <= 0f) {
                GiveResult();
                ResetGame();
            }
        }
    }

    public void AddScore() {
        currentScore++;
    }

    public void DeductScore() {
        currentScore--;
    }

    // Change the lighting colour based on the end result.
    void GiveResult() {
        if (currentScore >= targetScore) {
            resultLight.color = winColor;
        } else {
            resultLight.color = loseColor;
        }
        resultGiven = true;
    }

    public void StartGame() {
        resultGiven = false;
        gameEnded = false;
    }

    public void EndGame() {
        gameEnded = true;
    }

    // Reset the game's current score and time limit.
    void ResetGame() {
        currentScore = 0;
        currentTimer = timeLimit;
        resultLight.color = defaultLightColor;
    }

}
