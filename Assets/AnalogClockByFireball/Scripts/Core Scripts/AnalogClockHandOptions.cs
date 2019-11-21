using UnityEngine;

//Fireball Games * * * PetrZavodny.com

[CreateAssetMenu(fileName = "HandOptions", menuName = "Analog Clock by Fireball/Create Hand Options", order = 1)]
public class AnalogClockHandOptions : ScriptableObject
{
#pragma warning disable 649
    [SerializeField] bool isActive = true;
    [SerializeField] bool isShown = true;
    [SerializeField] bool isInSmoothMode = false;
    [SerializeField] float lag = 0f;
    
    public bool IsActive { get => isActive; set => isActive = value; }
    public bool IsShown { get => isShown; set => isShown = value; }
    public bool IsInSmoothMode { get => isInSmoothMode; set => isInSmoothMode = value; }
    public float Lag { get => lag; set => lag = value; }
#pragma warning restore 649
}
