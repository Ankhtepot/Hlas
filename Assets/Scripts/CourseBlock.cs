using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Constants;
using UnityEngine;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

[CreateAssetMenu(fileName = "CourseBlock", menuName = "Hlas/CourseBlock")]
public class CourseBlock : ScriptableObject
{
#pragma warning disable 649, 414
    [SerializeField] string nameOfBlock;
    [SerializeField] CourseId courseId;
    [SerializeField] AudioClip clip;

    public string NameOfBlock { get => nameOfBlock; }
    public CourseId CourseId { get => courseId; }
#pragma warning restore 649, 414

    public float GetClipLength()
    {
        return clip.length;
    }

    public AudioClip GetClip()
    {
        return clip;
    }
}
