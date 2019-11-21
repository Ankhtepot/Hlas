using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

public class PlaylistButton : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] PredefinedPlaylist predefinedForPlaylist;
    public Playlist playlist;
    [Header("Observed fields")]
    public Course course;

    public PredefinedPlaylist PredefinedForPlaylist { get => predefinedForPlaylist; }
#pragma warning restore 649, 414

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickListener);
    }

    void Update()
    {
        
    }

    public void OnClickListener()
    {
        course.OnPlaylistButtonClick(playlist);
    }
}
