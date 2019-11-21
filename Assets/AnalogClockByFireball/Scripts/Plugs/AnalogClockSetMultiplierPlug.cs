using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Fireball Games * * * PetrZavodny.com

public class AnalogClockSetMultiplierPlug : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] string[] AffectsClocksNames = new string[] { "" };
    [SerializeField] string PlugName = "";
    [SerializeField] public float NewMultiplierValue = 1;
    [SerializeField] Slider Slider;
    [SerializeField] int SliderMinValue = 0;
    [SerializeField] int SliderMaxValue = 100;
    [SerializeField] AnalogClockController[] AffectedClocks = new AnalogClockController[] { };
#pragma warning restore 649

    void Start()
    {
        assignAssets();
    }

    void Update()
    {
        
    }

    public void IniciateChangeAtAssignedClocks()
    {
        IniciateChangeAtAssignedClocks(NewMultiplierValue);
    }

    public void IniciateChangeAtAssignedClocks(float newValue)
    {
        AffectedClocks.ToList().ForEach(clock => clock.SetMultiplierValue(newValue));
    }

    private void TriggerMultiplierChange(float value)
    {
        if (Slider)
        {
            IniciateChangeAtAssignedClocks(value);
        }
    }

    private void assignAssets()
    {
        AffectedClocks = FindObjectsOfType<AnalogClockController>().Where(clock => AffectsClocksNames.Contains(clock.ClockName)).ToArray();
        if (AffectedClocks.Length == 0)
        {
            Debug.LogWarning($"AnalogClockSetMultiplierPlug \"{PlugName}\" has no AnalogClocks assigned to controll.");
        }

        if (Slider == null)
        {
            Slider = GetComponent<Slider>();
        }

        if (Slider != null)
        {
            Slider.minValue = SliderMinValue;
            Slider.maxValue = SliderMaxValue;
            Slider.value = 1;
            Slider.onValueChanged.AddListener(TriggerMultiplierChange);
        }
    }

    
}
