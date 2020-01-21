using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{

    Animator anim;
    public ScoreManager score;
    bool buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed) {
            anim.Play("Press");
            PressButton();
        }
    }

    void PressButton() {
        score.StartGame();
    }
}
