using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Fireball Games * * * PetrZavodny.com

public class AnalogClockSetTimePlug : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] string TargetClockName;
    [SerializeField] int newHours;
    [SerializeField] int newMinutes;
    [SerializeField] int newSeconds;
    [SerializeField] AnalogClockController TargetClock;
#pragma warning restore 649

    void Start()
    {
        TargetClock = FindObjectsOfType<AnalogClockController>().Where(clock => clock.ClockName == TargetClockName).ToList().First();
    }

    void Update()
    {

    }

    public void SetNewTime()
    {
        if (TargetClock)
        {
            //print($"Setting time to: h: {newHours} m: {newMinutes} s: {newSeconds}");
            TargetClock.SetCurrentTime(newHours, newMinutes, newSeconds);
        }
        else
        {
            Debug.LogWarning($"AnalogClockSetTImePlug: TargetClock \"{TargetClockName}\" not found.");
        }
    }
}
