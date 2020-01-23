using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    [SerializeField]
    int targetScore;
    public int currentScore;
    [SerializeField]
    GameObject startButton;
    [SerializeField]
    GameObject startButtonCanvas;
    [SerializeField]
    Text pauseButtonText;
    [SerializeField]
    Text resetButtonText;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text headerText;
    [SerializeField]
    Text resultText;
    [SerializeField]
    Text timerText;
    bool resetConfirm = false;
    [HideInInspector]
    public bool gameEnded;
    bool gamePaused;
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
    float secondDelay;
    float currentSecondDelay;
    bool firstMessageShown;

    [SerializeField]
    Color winColor;
    [SerializeField]
    Color loseColor;

    [SerializeField]
    AudioSource sfxAudio;
    AudioSource bgmAudio;
    [SerializeField]
    AudioClip winAudio;
    [SerializeField]
    AudioClip loseAudio;
    [SerializeField]
    AudioClip startAudio;
    [SerializeField]
    AudioClip endAudio;

    // Start is called before the first frame update
    void Start() {
        gameHasFinished = false;
        gameEnded = true;
        gamePaused = false;
        defaultLightColor = resultLight.color;
        currentTimer = timeLimit;
        currentResultDelay = resultDelay;
        currentResetDelay = resetDelay;
        currentSecondDelay = secondDelay;
        firstMessageShown = false;
        bgmAudio = GetComponent<AudioSource>();
        bgmAudio.Play();
    }

    // Update is called once per frame
    void Update() {
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
            StartCoroutine(audioFadeOut(bgmAudio, 1.5f));
            resultLight.color = Color.black;
            scoreText.gameObject.SetActive(false);
            headerText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            if (!firstMessageShown) {
                resultText.text = "Game ended";
                firstMessageShown = true;
            }

            currentSecondDelay -= Time.deltaTime;
            if (currentSecondDelay <= 0f) {
                resultText.text = "The results?";

                currentResultDelay -= Time.deltaTime;
                if (currentResultDelay <= 0f) {
                    GiveResult();
                }
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
            if (winAudio != null) {
                sfxAudio.PlayOneShot(winAudio);
            }
            resultText.text = "YOU WIN";
            resultLight.color = winColor;
        } else {
            if (loseAudio != null) {
                sfxAudio.PlayOneShot(loseAudio);
            }
            resultText.text = "YOU LOSE";
            resultLight.color = loseColor;
        }
        resultGiven = true;
    }

    public void StartGame() {
        if (startAudio != null) {
            sfxAudio.PlayOneShot(startAudio);
        }
        startButtonCanvas.SetActive(false);
        timerText.gameObject.SetActive(true);
        resultGiven = false;
        gameEnded = false;
    }

    public void EndGame() {
        gameEnded = true;
        gameHasFinished = true;
        if (endAudio != null) {
            sfxAudio.PlayOneShot(endAudio);
        }

    }

    // Reset the game's current score and time limit.
    void ResetGame() {
        StartCoroutine(audioFadeIn(bgmAudio, 1.5f));
        gameHasFinished = false;
        resultText.text = "";
        scoreText.gameObject.SetActive(true);
        headerText.gameObject.SetActive(true);
        currentScore = 0;
        currentTimer = timeLimit;
        currentResetDelay = resetDelay;
        currentResultDelay = resultDelay;
        currentSecondDelay = secondDelay;
        firstMessageShown = false;
        resultLight.color = defaultLightColor;
        startButtonCanvas.SetActive(true);
    }

    // Resets the game completely.
    public void HardResetGame() {
        if (resetConfirm == true) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        if (startAudio != null) {
            sfxAudio.PlayOneShot(startAudio);
        }
        resetConfirm = true;
        resetButtonText.text = "Are you sure?";
    }

    // Resumes or pauses the game.
    public void TogglePauseGame() {
        if (!gameEnded) {
            if (startAudio != null) {
                sfxAudio.PlayOneShot(startAudio);
            }
            if (!gamePaused) {
                Time.timeScale = 0;
                pauseButtonText.text = "Resume Game";
                gamePaused = true;
            } else {
                Time.timeScale = 1;
                pauseButtonText.text = "Pause Game";
                gamePaused = false;
            }
        } 
    }

    IEnumerator audioFadeOut(AudioSource audio, float fadeTime) {
        float startVolume = audio.volume;
        while (audio.volume > 0.4f) {
            audio.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    IEnumerator audioFadeIn(AudioSource audio, float fadeTime) {
        audio.volume = 0.4f;
        while (audio.volume < 0.8f) {
            audio.volume += 0.2f * Time.deltaTime / fadeTime;
            yield return null;
        }
    }


}
