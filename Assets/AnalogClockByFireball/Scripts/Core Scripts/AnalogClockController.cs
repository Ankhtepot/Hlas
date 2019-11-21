using Assets;
using Assets.AnalogClockByFireball.Scripts.Core_Scripts.classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//Fireball Games * * * PetrZavodny.com

public class AnalogClockController : MonoBehaviour
{
#pragma warning disable 649
    private const int MIN_MULTIPLIER = 0;
    private const int MAX_MULTIPLIER = 100;
    private const int MAX_STEP_PER_SECOND = 50;
    private const string ERROR_MESSAGE_SUFFIX_HANDS_ASSIGNMENT = " hand is set active but is not assigned.";

    [Header("Options:")]
    [SerializeField] public string ClockName = "";
    [SerializeField] int HoursInACycle = 12;
    [SerializeField] int MinutesInAnHour = 60;
    [SerializeField] float SecondsInAMinute = 60;
    [Range(-MAX_STEP_PER_SECOND, MAX_STEP_PER_SECOND)] [SerializeField] int StepsPerOneSecond = 1;
    [Range(MIN_MULTIPLIER, MAX_MULTIPLIER)] [SerializeField] float Multiplier = 1;
    [Header("Round output")]
    [SerializeField] int RoundTo = 2;
    [SerializeField] bool IsClockActive = true;
    public bool AllowOnTickEventToFire = false;
    [Space(5)]
    [Header("Not Usable At Runtime:")]
    [SerializeField] int StartHours = 0;
    [SerializeField] int StartMinutes = 0;
    [SerializeField] double StartSeconds = 0;
    [Space(5)]
    [Header("Hands Cache")]
    [SerializeField] HandAssigningMethod SearchForHands = HandAssigningMethod.InChildren;
    [SerializeField] GameObject ExternalParent;
    [SerializeField] List<AnalogClockHandController> Hands = new List<AnalogClockHandController>();
    [Header("Fields Overview:")]
    [SerializeField] int currentHours;
    [SerializeField] int currentMinutes;
    [SerializeField] double currentSeconds;
    [SerializeField] double currentWholeCircleTime;
    [SerializeField] double totalSecondsInCycle;
    [Header("Events")]
    [SerializeField] public UnityEvent PlainOnTick;
    [SerializeField] public OnTickEvent OnTick;
    [SerializeField] public OnHoursChangeEvent OnHoursChange;
    [SerializeField] public OnMinutesChangeEvent OnMinutesChange;
    [SerializeField] public OnSecondsChangeEvent OnSecondsChange;

    private float HoursStep;
    private float MinutesStep;
    private float SecondsStep;
    private CustomTime time;

    private bool isTickOffCooldown = true;
    private bool activeStateSetFromMultiplier = false;


    public int CurrentHours { get => currentHours; }
    public int CurrentMinutes { get => currentMinutes; }
    public double CurrentSeconds { get => currentSeconds;  }
    public double CurrentWholeCircleTime { get => currentWholeCircleTime; }

    [System.Serializable] public class OnTickEvent : UnityEvent<CustomTime> { }
    [System.Serializable] public class OnHoursChangeEvent : UnityEvent<int> { }
    [System.Serializable] public class OnMinutesChangeEvent : UnityEvent<int> { }
    [System.Serializable] public class OnSecondsChangeEvent : UnityEvent<double> { }


    [System.Serializable]
    public enum HandShows
    {
        Hours,
        Minutes,
        Seconds
    }

    [System.Serializable]
    public enum HandAssigningMethod
    {
        InChildren,
        InChildrenByName,
        InExternalObjectChildren,
        GloballyByName,
        Globally,
        DontUseHands
    }
#pragma warning restore 649

    void Start()
    {
        buildHandsCache();

        time = new CustomTime(HoursInACycle, MinutesInAnHour, SecondsInAMinute);

        StructuredTime normalizedStartTime = time.GetNormalizedStructuredTime(new StructuredTime(StartHours, StartMinutes, StartSeconds), HoursInACycle, MinutesInAnHour, SecondsInAMinute);

        initializeClock(normalizedStartTime);
    }


    void Update()
    {
        if (isTickOffCooldown && IsClockActive && !activeStateSetFromMultiplier)
        {
            StartCoroutine(doTick());

            PlainOnTick.Invoke();
            if (AllowOnTickEventToFire)
            {
                OnTick.Invoke(GetCurrentTime());
            }

            moveHandsToNewPositionInSteps();
        }
    }

    public void SetTimeFramework(int hoursInCycle, int minutesInHour, float secondsInMinute, int startHours, int startMinutes, double startSeconds, bool activateUpponChange = true)
    {
        IsClockActive = false;

        StructuredTime normalizedStartTime = time.GetNormalizedStructuredTime(new StructuredTime(startHours, startMinutes, (float)startSeconds), hoursInCycle, minutesInHour, secondsInMinute);

        HoursInACycle = hoursInCycle;
        MinutesInAnHour = minutesInHour;
        SecondsInAMinute = secondsInMinute;

        initializeClock(normalizedStartTime);

        IsClockActive = activateUpponChange;
    }

    public double SetCurrentTime(int hours, int minutes, float seconds)
    {
        var originalActiveState = IsClockActive;
        IsClockActive = false;
        currentWholeCircleTime = time.SetTime(hours, minutes, seconds);
        setCurrentTimeFields();
        moveHandsToNewPositionSmoothly();
        IsClockActive = originalActiveState;

        return CurrentWholeCircleTime;
    }

    public void SetMultiplierValue(float newValue)
    {
        Multiplier = Mathf.Clamp(newValue, MIN_MULTIPLIER, MAX_MULTIPLIER);
    }

    public void SetActive(bool newState)
    {
        this.IsClockActive = newState;
    }

    public CustomTime GetCurrentTime()
    {
        return CustomTime.getCopyOf(time);
    }

    private void initializeClock(StructuredTime normalizedStartTime)
    {
        time = new CustomTime(HoursInACycle, MinutesInAnHour, SecondsInAMinute);

        StartHours = normalizedStartTime.Hours;
        StartMinutes = normalizedStartTime.Minutes;
        StartSeconds = normalizedStartTime.Seconds;

        HoursStep = HoursInACycle != 0 ? 360f / HoursInACycle : 0;
        MinutesStep = MinutesInAnHour != 0 ? 360f / MinutesInAnHour : 0;
        SecondsStep = SecondsInAMinute != 0 ? 360f / SecondsInAMinute : 0;

        totalSecondsInCycle = (HoursInACycle == 0 ? 1 : HoursInACycle) * (MinutesInAnHour == 0 ? 1 : MinutesInAnHour) * (SecondsInAMinute == 0 ? 1 : SecondsInAMinute);

        currentWholeCircleTime = time.SetTime(StartHours, StartMinutes, StartSeconds);
        setCurrentTimeFields();
        moveHandsToNewPositionInSteps();
    }

    private void buildHandsCache()
    {
        if(SearchForHands == HandAssigningMethod.DontUseHands)
        {
            return;
        }

        Hands = searchForHandsByMethod(SearchForHands);

        if (Hands.Any())
        {
            checkPrerequisites(Hands);
        }
        else
        {
            Debug.LogWarning("AnalogClock: There are no Hands set to be used.");
        }
    }

    private List<AnalogClockHandController> searchForHandsByMethod(HandAssigningMethod method)
    {
        switch (method)
        {
            case HandAssigningMethod.InChildren: return GetComponentsInChildren<AnalogClockHandController>().ToList();
            case HandAssigningMethod.InChildrenByName: return GetComponentsInChildren<AnalogClockHandController>().Where(hand => hand.BelongsToClock == this.ClockName).ToList();
            case HandAssigningMethod.Globally: return Resources.FindObjectsOfTypeAll(typeof(AnalogClockHandController)).Select(hand => (AnalogClockHandController)hand).ToList();
            case HandAssigningMethod.GloballyByName: return Resources.FindObjectsOfTypeAll(typeof(AnalogClockHandController))
                    .Select(hand => (AnalogClockHandController)hand)
                    .Where(hand => hand.BelongsToClock == this.ClockName).ToList();
            case HandAssigningMethod.InExternalObjectChildren: return assignHandsFromExternalObject();
        }

        return new List<AnalogClockHandController>() { };
    }

    private List<AnalogClockHandController> assignHandsFromExternalObject()
    {
        if (ExternalParent)
        {
            return ExternalParent.GetComponentsInChildren<AnalogClockHandController>().ToList();
        }

        else return new List<AnalogClockHandController>() { };
    }

    private void moveHandsToNewPositionInSteps()
    {
        Hands.ForEach(hand => setHandPositionInAStep(hand));
    }

    private void setHandPositionInAStep(AnalogClockHandController hand)
    {
        if (hand.IsActive())
        {
            if (!hand.IsShown())
            {
                return;
            }

            var handValue = countHandValue(hand);
            hand.transform.localRotation = Quaternion.Euler((float)(getStepValue(hand.HandShows) * handValue), 0, 0); 
        }
    }

    private void moveHandsToNewPositionSmoothly()
    {
        Hands.ForEach(hand => 
        {
            StartCoroutine(moveHandToNewPositionSmoothly(hand, (float)(getStepValue(hand.HandShows) * countHandValue(hand))));
        });
    }

    private IEnumerator moveHandToNewPositionSmoothly(AnalogClockHandController hand, float finalAngle)
    {
        float moveSpeed = 0.01f;
        float finalAllowedOfset = 0.05f;

        while (Mathf.Abs(hand.transform.localRotation.x) + finalAllowedOfset < Quaternion.Euler(finalAngle, 0, 0).x)
        {
            hand.transform.localRotation = Quaternion.Slerp(hand.transform.localRotation, Quaternion.Euler(finalAngle, 0, 0), moveSpeed * Time.time);
            yield return null;
        }
        
        if (hand.transform.localRotation.x < 0)
        {
            float angleX = Mathf.Repeat(hand.transform.localRotation.eulerAngles.x, 360.0f);
            print($"Hand: {hand.HandName}.EulerAngles are {hand.transform.rotation.eulerAngles.x}, anglex: {angleX}");
            hand.transform.localRotation = Quaternion.Euler(angleX, 0, 0);
        }
        yield return null;
        print("finishing couroutine");
    }

    private IEnumerator doTick()
    {
        if (Multiplier == 0)
        {
            activeStateSetFromMultiplier = true;
            yield return null;
        }

        activeStateSetFromMultiplier = false;
        isTickOffCooldown = false;
        float checkedStep = StepsPerOneSecond == 0 ? float.MinValue : (float)StepsPerOneSecond;

        yield return new WaitForSecondsRealtime(1f / Mathf.Abs(checkedStep));
        currentWholeCircleTime = time.AddTime((1f / checkedStep) * Mathf.Clamp(Multiplier, MIN_MULTIPLIER, MAX_MULTIPLIER));

        setCurrentTimeFields();

        isTickOffCooldown = true;
    }

    private void checkPrerequisites(List<AnalogClockHandController> hands)
    {
        hands.ForEach(hand => checkHandAvailability(hand));
    }

    private bool checkHandAvailability(AnalogClockHandController hand)
    { 
        if (hand.IsActive() && !hand.HandObject && SearchForHands != HandAssigningMethod.DontUseHands)
        {
            hand.SetActive(false);
            throw new MissingComponentException($"{hand.HandName} {ERROR_MESSAGE_SUFFIX_HANDS_ASSIGNMENT} ; DeActivating the hand.");
        }

        return true;
    }

    private double countHandValue(AnalogClockHandController hand)
    {
        switch (hand.HandShows)
        {
            case HandShows.Hours:
                {
                    float newHourValue = (hand.IsInSmoothMode() ? (float)time.Minutes / (float)MinutesInAnHour : 0) + time.Hours + hand.GetLag();
                    return newHourValue;
                }; 
            case HandShows.Minutes:
                {
                    float newMinutesValue = (hand.IsInSmoothMode() ? (float)time.Seconds / (float)SecondsInAMinute : 0) + time.Minutes + hand.GetLag();
                    return newMinutesValue;
                }
            case HandShows.Seconds:
                {
                    double newSecondsValue = (hand.IsInSmoothMode() ? time.Seconds : (int)time.Seconds) + hand.GetLag();                  
                    return newSecondsValue;
                }
            default: return 0;
        }
    }

    private double getStepValue(HandShows shows)
    {
        switch (shows)
        {
            case HandShows.Hours: return HoursStep;
            case HandShows.Minutes: return MinutesStep; 
            case HandShows.Seconds: return SecondsStep;
            default: return 6d;
        }
    }

    private void setCurrentTimeFields()
    {
        if (currentHours != time.Hours)
        {
            OnHoursChange.Invoke(time.Hours);
        }

        if (currentMinutes != time.Minutes)
        {
            OnMinutesChange.Invoke(time.Minutes);
        }

        if (currentSeconds != time.Seconds)
        {
            OnSecondsChange.Invoke(Math.Round(time.Seconds, RoundTo)); 
        }

        currentHours = time.Hours;
        currentMinutes = time.Minutes;
        currentSeconds = Math.Round(time.Seconds, RoundTo);
    }
}
