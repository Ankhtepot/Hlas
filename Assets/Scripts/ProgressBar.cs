﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    [SerializeField] float fillSpeed = 0.01f;
    [SerializeField] Image bar;


    [SerializeField] private float targetValue;

    private void Start() {
        targetValue = bar.fillAmount;
    }

    private void Update() {
        if(bar.fillAmount != targetValue)
        { 
            bar.fillAmount += bar.fillAmount < targetValue ? fillSpeed : -fillSpeed;
        }
    }

    public void UpdateBar(float value) {
        targetValue += Mathf.Clamp(value, 0, 1);
    }

    //public void EnableVisuals() {
    //    bar.enabled = true;
    //    frame.enabled = true;
    //}

    //public void DisableVisuals() {
    //    bar.enabled = false;
    //    frame.enabled = false;
    //}
}
