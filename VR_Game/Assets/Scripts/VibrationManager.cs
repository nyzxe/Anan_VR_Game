﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour {
    public static VibrationManager singleton;

    // Start is called before the first frame update
    void Start() {
        if (singleton && singleton != this) {
            Destroy(this);
        } else {
            singleton = this;
        }
    }

    public void TriggerVibration(AudioClip vibrationAudio, OVRInput.Controller controller) {
        OVRHapticsClip clip = new OVRHapticsClip(vibrationAudio);

        if (controller == OVRInput.Controller.LTouch) {
            OVRHaptics.LeftChannel.Preempt(clip);
        } else if (controller == OVRInput.Controller.RTouch) {
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }

    public void TriggerVibration(int iteration, int frequency, int strength, OVRInput.Controller controller) {
        OVRHapticsClip clip = new OVRHapticsClip();

        for (int i = 0; i < iteration; i++) {
            clip.WriteSample(i % frequency == 0 ? (byte)strength : (byte)0);
        }

        if (controller == OVRInput.Controller.LTouch) {
            OVRHaptics.LeftChannel.Preempt(clip);
        } else if (controller == OVRInput.Controller.RTouch) {
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }
}