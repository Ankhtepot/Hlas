using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Fireball Games * * * PetrZavodny.com

public class AnalogClockExternalSourcePlug : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] string[] TargetHandsNames = new string[] {""};
    [SerializeField] float NewAngle;
    [SerializeField] int StepsInCycle;
    [SerializeField] bool DeactivateTargetHands = true;
    [SerializeField] List<AnalogClockHandController> TargetHands = new List<AnalogClockHandController>() { };
#pragma warning restore 649

    void Start()
    {
        TargetHands = FindObjectsOfType<AnalogClockHandController>().Where(hand => TargetHandsNames.Contains(hand.HandName)).ToList();
    }

    void Update()
    {
        
    }

    public void SetAndTriggerChange()
    {
        //Here put your code to determinate new Angle

        //Example

        //if (TargetHands.Any())
        //{
        //    float originalAngle = TargetHands.First().GetCurrentHandAngle();
        //    newAngle = originalAngle + 10;
        //}

        SetTargetHandsValue();
    }

    public float GetCurrentHandAngle(AnalogClockHandController hand)
    {
        return hand.GetCurrentHandAngle();
    }

    public List<AnalogClockHandController> GetTargetHands()
    {
        return TargetHands;
    }

    private void SetTargetHandsValue()
    {
        TargetHands.ForEach(hand =>
        {
            if(DeactivateTargetHands)
            {
                hand.SetActive(false);
            }

            hand.SetHandAngleExternaly(NewAngle, StepsInCycle);
        });
    }
}
