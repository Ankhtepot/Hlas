using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    [SerializeField] float fillSpeed = 0.01f;
    [SerializeField] float toleratedDif = 0.02f;
    [SerializeField] Image bar;
    [SerializeField] Slider slider;
    [Header("")]
    [SerializeField] private float targetValue;

    private void Start() {
        targetValue = bar.fillAmount;
        if (slider)
        {
            slider.onValueChanged.AddListener(OnSliderChange);
        }
    }

    private void Update() {
        if (!(bar.fillAmount >= targetValue - toleratedDif && bar.fillAmount <= targetValue + toleratedDif))
        {
            bar.fillAmount += bar.fillAmount < targetValue ? fillSpeed : -fillSpeed;
        }
    }

    public void UpdateBar(float value) {
        targetValue += Mathf.Clamp(value, 0, 1);
    }

    public void SetValueReverseOne(float value)
    {
        OnSliderChange(value);
    }

    public void SetValueDirectly(float value)
    {
        targetValue = Mathf.Clamp(value, 0, 1);
    }

    private void OnSliderChange(float value)
    {
        targetValue = 1 - Mathf.Clamp(value, 0, 1);
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
