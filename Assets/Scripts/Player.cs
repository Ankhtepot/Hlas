using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Fireball Games * * * PetrZavodny.com

public class Player : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] ProgressBar progressBar;
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI courseText;
    [SerializeField] TextMeshProUGUI playingInfoText;
    [SerializeField] TextMeshProUGUI currentTimeText;
    [SerializeField] TextMeshProUGUI totalTimeText;
    [SerializeField] Slider slider;
    [SerializeField] Button backToCourseButton;
    [SerializeField] Button backToHomeButton;
    [SerializeField] AnalogClockController clock;
    [Header("Observed fields")]
    [SerializeField] Playlist playlist;
    [SerializeField] CourseConfiguration configuration;
    [SerializeField] float totalPlaylistTime;
    [SerializeField] ProgramController PC;
    [SerializeField] Course course;
    [SerializeField] bool validated = false;
#pragma warning restore 649, 414

    void Start()
    {
        initilize();        
    }

    void Update()
    {
        
    }

    public void FetchCourseScreen()
    {
        clear();
        course.SetScreen(configuration);
    }

    public void FetchMainScreen()
    {
        clear();
        PC.SetMainScreen();
    }

    private void clear()
    {
        animator.SetBool(triggers.SHOW, false);
        clock.SetActive(false);
    }

    public void SetScreen(CourseConfiguration configuration, Playlist playlist)
    {
        if (!validated) return;

        this.configuration = configuration;
        this.playlist = playlist;
        totalPlaylistTime = playlist.GetTotalDuration();

        clock.SetTimeFramework(0, 0, totalPlaylistTime, 0, 0, 0d, false);

        courseText.text = configuration.CourseName;
        playingInfoText.text = playlist.playlistName;
        backToCourseButton.image.color = configuration.ButtonBackgroundColor;
        backToHomeButton.image.color = configuration.ButtonBackgroundColor;
        currentTimeText.text = "0:00";
        totalTimeText.text = timeTextFromCycleTime(totalPlaylistTime);

        animator.SetBool(triggers.SHOW, true);
    }

    private string timeTextFromCycleTime(float cycleTime)
    {
        int minutes = (int)(cycleTime / 60);
        int seconds = (int)cycleTime - (int)(cycleTime / minutes);
        return $"{minutes}:{seconds}";
    }

    private void initilize()
    {
        if(
            !progressBar
            || !courseText
            || !playingInfoText
            || !currentTimeText
            || !totalTimeText
            || !slider
            || !backToCourseButton
            || !backToHomeButton
            || !clock
            )
        {            
            Debug.LogWarning("Player: one or more components are not assigned.");
            return;
        }

        PC = FindObjectOfType<ProgramController>();
        course = FindObjectOfType<Course>();

        validated = true;
    }
}
