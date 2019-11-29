using Assets.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

[CreateAssetMenu(fileName = "Configuration", menuName = "Hlas/Configuration")]
public class CourseConfiguration : ScriptableObject
{
#pragma warning disable 649, 414
    public string CourseName;
    [SerializeField] CourseId courseId;
    [SerializeField] bool purchased;
    [SerializeField] int prize;
    [Header("Visual Options")]
    [SerializeField] Color buttonBackgroundColor;
    [SerializeField] Color buttonTextColor;
    public Color ButtonBackgroundColor { get => buttonBackgroundColor; set => buttonBackgroundColor = value; }
    public Color ButtonTextColor { get => buttonTextColor; set => buttonBackgroundColor = value; }
    public CourseId CourseId { get => courseId; }
    public bool Purchased { get => purchased; }
    public int Prize { get => prize; }
#pragma warning restore 649, 414

    public void SetPurchased(bool isPurchased)
    {
        purchased = isPurchased;
    }
}
