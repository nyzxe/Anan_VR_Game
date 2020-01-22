using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
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
    [SerializeField]
    Text timerText;
    [HideInInspector]
    public bool gameEnded;
    bool gameHasFinished;
    bool resultGiven;
    public float timeLimit;
    float currentTimer;
    int intTimer;
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
        gameHasFinished = false;
        gameEnded = true;
        defaultLightColor = resultLight.color;
        currentTimer = timeLimit;
        currentResultDelay = resultDelay;
        currentResetDelay = resetDelay;
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is ongoing, count down the timer.
        if (!gameEnded) {
            currentTimer -= Time.deltaTime;
            intTimer = Mathf.FloorToInt(currentTimer);
            timerText.text = intTimer.ToString();
            scoreText.text = currentScore.ToString();
            if (currentTimer <= 0f) {
                EndGame();
            }

        // If the game has ended, count down the result delay.
        } else if (gameEnded && !resultGiven && gameHasFinished) {
            resultLight.color = Color.black;
            resultText.text = "The results?";
            scoreText.gameObject.SetActive(false);
            headerText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            currentResultDelay -= Time.deltaTime;
            if (currentResultDelay <= 0f) {
                GiveResult();
            }
        // If the game has ended and the result is given, reset the game.
        } else if (gameEnded && resultGiven && gameHasFinished) {
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
        timerText.gameObject.SetActive(true);
        resultGiven = false;
        gameEnded = false;
    }

    public void EndGame() {
        gameEnded = true;
        gameHasFinished = true;
    }

    // Reset the game's current score and time limit.
    void ResetGame() {
        gameHasFinished = false;
        resultText.text = "";
        scoreText.gameObject.SetActive(true);
        headerText.gameObject.SetActive(true);
        currentScore = 0;
        currentTimer = timeLimit;
        currentResetDelay = resetDelay;
        currentResultDelay = resultDelay;
        resultLight.color = defaultLightColor;
        startButton.SetActive(true);
    }

    


}
