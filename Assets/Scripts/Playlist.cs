using Assets.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

[CreateAssetMenu(fileName = "Playlist", menuName = "Hlas/Playlist")]
public class Playlist : ScriptableObject
{
#pragma warning disable 649, 414
    public string playlistName;
    [SerializeField] CourseId courseId;
    [SerializeField] PredefinedPlaylist predefinedPlaylist;
    [SerializeField] List<CourseBlock> blocks = new List<CourseBlock>();

    public PredefinedPlaylist PredefinedPlaylist { get => predefinedPlaylist; }
    public CourseId CourseId { get => courseId; }
#pragma warning restore 649, 414

    public CourseBlock[] GetBlocks()
    {
        return blocks.ToArray();
    }

    public float GetTotalDuration()
    {
        return blocks.Aggregate(0f, (prev, curr) => prev + curr.GetClipLength());
    }
}
