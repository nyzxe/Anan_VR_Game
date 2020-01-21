using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    int targetScore;
    public int currentScore;
    [SerializeField]
    GameObject startButton;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text headerText;
    [SerializeField]
    Text resultText;
    [HideInInspector]
    public bool gameEnded;
    bool resultGiven;
    public float timeLimit;
    float currentTimer;
    public Light resultLight;
    Color defaultLightColor;

    [SerializeField]
    float resultDelay;
    float currentResultDelay;

    [SerializeField]
    float resetDelay;
    float currentResetDelay;

    [SerializeField]
    Color winColor;
    [SerializeField]
    Color loseColor;

    // Start is called before the first frame update
    void Start()
    {
        gameEnded = true;
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
            resultLight.color = Color.white;
            scoreText.gameObject.SetActive(false);
            headerText.gameObject.SetActive(false);
            currentResultDelay -= Time.deltaTime;
            if (currentResultDelay <= 0f) {
                GiveResult();
            }
        // If the game has ended and the result is given, reset the game.
        } else if (gameEnded && resultGiven) {
            currentResetDelay -= Time.deltaTime;
            if (currentResetDelay <= 0f) {
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
            resultText.text = "YOU WIN";
            resultLight.color = winColor;
        } else {
            resultText.text = "YOU LOSE";
            resultLight.color = loseColor;
        }
        resultGiven = true;
    }

    public void StartGame() {
        startButton.SetActive(false);
        resultGiven = false;
        gameEnded = false;
    }

    public void EndGame() {
        gameEnded = true;
    }

    // Reset the game's current score and time limit.
    void ResetGame() {
        resultText.text = "";
        scoreText.gameObject.SetActive(true);
        headerText.gameObject.SetActive(true);
        currentScore = 0;
        currentTimer = timeLimit;
        resultLight.color = defaultLightColor;
        startButton.SetActive(true);
    }

    


}
