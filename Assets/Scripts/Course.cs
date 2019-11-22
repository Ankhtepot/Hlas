using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Fireball Games * * * PetrZavodny.com

public class Course : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] Animator courseScrAnimator;
    [SerializeField] ProgramController PC;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] PlaylistStore playlistStore;
    [SerializeField] Player player;
    [Header("Observed fields")]
    [SerializeField] CourseConfiguration configuration;
    [SerializeField] List<Button> buttons = new List<Button>();
    [SerializeField] bool validated = false;
    //[SerializeField] List<Playlist> playlists = new List<Playlist>();
#pragma warning restore 649, 414

    void Start()
    {
        initialized();
    }

    void Update()
    {
        
    }

    public void SetScreen(CourseConfiguration configuration)
    {
        if (!validated) return;

        this.configuration = configuration;
        setScreen();
        showScreen(true);
    }

    public void BackButtonClick()
    {
        FetchMainScreen();
    }

    private void FetchMainScreen()
    {
        showScreen(false);
        PC.SetMainScreen();
    }

    private void initialized()
    {
        if(
            !courseScrAnimator
            || !PC
            || !titleText
            || !playlistStore
            || !player
            )
        {
            Debug.LogWarning("Course: one or more components are not assigned.");
            return;
        }

        validated = true;
    }

    public void OnPlaylistButtonClick(Playlist playlist)
    {
        //print("Should set player screen");
        showScreen(false);
        player.SetScreen(configuration, playlist);
    }

    private void setScreen()
    {
        if (!validated) return;

        buttons = new List<Button>(GetComponentsInChildren<Button>());

        buttons.ForEach(button =>
        {
            button.image.color = configuration.ButtonBackgroundColor;
            button.GetComponentInChildren<TextMeshProUGUI>().color = configuration.ButtonTextColor;
            PlaylistButton plButton = button.GetComponent<PlaylistButton>();
            if (plButton)
            {
                plButton.course = this;
                assignPlaylistToButton(plButton);
            }
        });

        titleText.color = configuration.ButtonTextColor;
        titleText.text = configuration.CourseName;
    }

    private void showScreen(bool show)
    {
        courseScrAnimator.SetBool(triggers.SHOW, show);
    }

    private void assignPlaylistToButton(PlaylistButton button)
    {
        Playlist foundPlaylist = playlistStore.store.Where(pl => pl.CourseId == configuration.CourseId && pl.PredefinedPlaylist == button.PredefinedForPlaylist).FirstOrDefault();
        if (foundPlaylist != null)
        {
            button.playlist = foundPlaylist;
        }
    }
}
