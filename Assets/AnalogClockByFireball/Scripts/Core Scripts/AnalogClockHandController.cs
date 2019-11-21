using System.Collections;
using UnityEngine;
using static AnalogClockController;

//Fireball Games * * * PetrZavodny.com

public class AnalogClockHandController : MonoBehaviour
{
#pragma warning disable 649
    [Header("Hand personalization:")]
    public string HandName = "unnamed hand";
    public string BelongsToClock = "";
    public HandShows HandShows;
    [SerializeField] float overrideLag = 0f;
    [SerializeField] float continuousLagPerStep = 0f;
    [Header("Options (only at runtime):")]
    [SerializeField] bool OptionsIsActive;
    [SerializeField] bool OptionsIsShown;
    [SerializeField] bool OptionsIsInSmoothMode;
    [SerializeField] float OptionsLag;
    [Header("Mandatory assignies")]
    [SerializeField] AnalogClockHandOptions Options;
    [SerializeField] public GameObject HandObject;

    //Cached properties
    private bool oldOptionsIsActive;
    private bool oldOptionsIsInSmoothMode;
    private bool oldOptionsIsShown;
    private float oldLag;
    private bool optionsChangeCheckOffCooldown = true;
    private float optionsChangeCheckPeriod = 1f;
    private bool continuosLagTickOffCooldown = true;
    private const float continuousLagPeriod = 1f;
#pragma warning restore 649

    void Start()
    {
        checkPrerequisites();

        makeStandaloneOptions();
    }

    void Update()
    {
        if (optionsChangeCheckOffCooldown)
        {
            StartCoroutine(checkOptionsChange());
        }

        if (continuousLagPerStep != 0 && continuosLagTickOffCooldown)
        {
            StartCoroutine(addContinuosLag());
        }
    }

    public bool IsActive()
    {
        if (Options)
        {
            return Options.IsActive;
        }

        return false;
    }

    public void SetActive(bool isActive)
    {
        Options.IsActive = isActive;

        OptionsIsActive = isActive;
        oldOptionsIsActive = isActive;
    }

    public bool IsInSmoothMode()
    {
        if (Options)
        {
            return Options.IsInSmoothMode;
        }

        return false;
    }

    public void SetSmoothMode(bool isInSmoothMode)
    {
        Options.IsInSmoothMode = isInSmoothMode;

        OptionsIsInSmoothMode = isInSmoothMode;
        oldOptionsIsInSmoothMode = isInSmoothMode;
    }


    public bool IsShown()
    {
        if (Options)
        {
            return Options.IsShown;
        }

        return false;
    }

    public void SetShown(bool isShown)
    {
        if (isShown)
        {
            Options.IsShown = true;
            HandObject.SetActive(true);
        }
        else
        {
            Options.IsShown = false;
            HandObject.SetActive(false);
        }

        OptionsIsShown = isShown;
        oldOptionsIsShown = isShown;
    }

    public float GetLag()
    {
        if (overrideLag != 0f)
        {
            return overrideLag;
        }

        return OptionsLag;
    }

    public void SetLag(float lag)
    {
        Options.Lag = lag;
        OptionsLag = lag;
        oldLag = lag;
    }

    public float GetCurrentHandAngle()
    {
        return transform.localRotation.x;
    }

    public void SetHandAngleExternaly(float newValue, int stepsInCycle = 360)
    {
        Options.IsActive = false;
        transform.localRotation = Quaternion.Euler(newValue + (GetLag() * (360 / stepsInCycle)), 0, 0);
    }

    private IEnumerator checkOptionsChange()
    {
        optionsChangeCheckOffCooldown = false;
        if (oldOptionsIsActive != OptionsIsActive) SetActive(OptionsIsActive);
        if (oldOptionsIsInSmoothMode != OptionsIsInSmoothMode) SetSmoothMode(OptionsIsInSmoothMode);
        if (oldOptionsIsShown != OptionsIsShown) SetShown(OptionsIsShown);
        if (oldLag != OptionsLag) SetLag(OptionsLag);
        yield return new WaitForSeconds(optionsChangeCheckPeriod);
        optionsChangeCheckOffCooldown = true;
    }

    private IEnumerator addContinuosLag()
    {
        continuosLagTickOffCooldown = false;
        yield return new WaitForSecondsRealtime(continuousLagPeriod);
        overrideLag += continuousLagPerStep;
        continuosLagTickOffCooldown = true;
    }

    private void checkPrerequisites()
    {
        if (Options == null)
        {
            var defaultOptions = ScriptableObject.CreateInstance<AnalogClockHandOptions>();
            defaultOptions.IsActive = true;
            defaultOptions.IsShown = true;
            defaultOptions.IsInSmoothMode = true;
            defaultOptions.Lag = 0;

            Options = defaultOptions;
        }
    }

    private void makeStandaloneOptions()
    {
        if (Options)
        {
            var internalOptions = Instantiate(Options, transform);
            Options = internalOptions;
        }

        OptionsIsActive = Options.IsActive;
        OptionsIsInSmoothMode = Options.IsInSmoothMode;
        OptionsIsShown = Options.IsShown;
        OptionsLag = Options.Lag;
    }
}
