using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] Button PlayButton;
    [SerializeField] Button PauseButton;
    [SerializeField] Button StopButton;
    [SerializeField] AnalogClockController clock;
    //[SerializeField] AudioSource player;
    [Header("Observed fields")]
    [SerializeField] bool clipsArePlaying = false;
    [SerializeField] List<AudioClip> audioClipList;
    [SerializeField] AudioSource[] audioSourceArray;
    [SerializeField] Playlist playlist;
    [SerializeField] CourseConfiguration configuration;
    [SerializeField] float totalPlaylistTime;
    [SerializeField] ProgramController PC;
    [SerializeField] Course course;
    [SerializeField] bool validated = false;

    private double nextStartTime = 0d;
    private bool checkClipsCooldownIsOff = true;
    public const string ZERO_HOURS = "00:00";
#pragma warning restore 649, 414

    void Start()
    {
        initilize();        
    }

    

    void Update()
    {
        if(clipsArePlaying && checkClipsCooldownIsOff)
        {
            StartCoroutine(checkIfAllClipsFinished());
        }
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
        StopPlaying();
        animator.SetBool(triggers.SHOW, false);
        audioClipList.Clear();
    }

    public void SetScreen(CourseConfiguration configuration, Playlist playlist)
    {
        if (!validated) return;

        this.configuration = configuration;
        this.playlist = playlist;
        totalPlaylistTime = playlist.GetTotalDuration();
        fillAudioClipArray(playlist);

        clock.SetTimeFramework(0, 0, totalPlaylistTime, 0, 0, 0d, false);

        courseText.text = configuration.CourseName;
        playingInfoText.text = playlist.playlistName;
        backToCourseButton.image.color = configuration.ButtonBackgroundColor;
        backToHomeButton.image.color = configuration.ButtonBackgroundColor;
        currentTimeText.text = ZERO_HOURS;
        totalTimeText.text = timeTextFromCycleTime(totalPlaylistTime);

        animator.SetBool(triggers.SHOW, true);
    }

    private void fillAudioClipArray(Playlist playlist)
    {
        playlist.blocks.ForEach(block =>
        {
            if(block.GetClip())
            {
                audioClipList.Add(block.GetClip());
            }
        });
    }

    private string timeTextFromCycleTime(float cycleTime)
    {
        int minutes = (int)(cycleTime / 60f);
        int seconds = (int)(cycleTime - (minutes * 60f));
        //print($"[timeTextFromCycleTime] minutes: {minutes} seconds: {seconds}");
        return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }

    public void StartPlaying()
    {
        if(audioClipList.Count == 0)
        {
            Debug.LogWarning("Player: there are no clips to play.");
            return;
        }

        nextStartTime = 0;

        PlayButton.interactable = false;
        PauseButton.interactable = true;
        StopButton.interactable = true;

        //print("Starting playing");

        if (AudioListener.pause == false)
        {
            clock.SetCurrentTime(0, 0, 0f);

            audioSourceArray = new AudioSource[audioClipList.Count];
            for (int i = 0; i < audioClipList.Count; i++)
            {
                audioSourceArray[i] = gameObject.AddComponent<AudioSource>();
                audioSourceArray[i].clip = audioClipList[i];
                audioSourceArray[i].PlayScheduled(AudioSettings.dspTime + nextStartTime);
                nextStartTime += audioClipList[i].length;
            } 
        }
        else
        {
            AudioListener.pause = false;
        }

        clock.SetActive(true);
        clipsArePlaying = true;
    }

    public void PausePlaying()
    {
        //print("Pausing playing");

        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = true;

        clock.SetActive(false);
        AudioListener.pause = true;
    }

    public void StopPlaying()
    {
        //print("Stopping playing");

        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = false;

        foreach (var source in audioSourceArray)
        {
            source.Stop();            
        }

        clipsArePlaying = false;
        
        clock.SetActive(false);
        progressBar.SetValueDirectly(1f);
        AudioListener.pause = false;
        checkClipsCooldownIsOff = true;
        StopAllCoroutines();
        currentTimeText.text = ZERO_HOURS;
    }

    private IEnumerator checkIfAllClipsFinished()
    {
        //print("Checking is clips are playing");
        checkClipsCooldownIsOff = false;
        yield return new WaitForSeconds(1f);

        bool noSourcePlaying = true;

        foreach (var source in audioSourceArray)
        {
            if (source.isPlaying)
            {
                //print(source.name + " is playing");
                noSourcePlaying = false;
            }
        }

        if(noSourcePlaying)
        {
            //print("noSourcePlaying = true");
            clipsArePlaying = false;

            clock.SetActive(false);

            if (!AudioListener.pause)
            {
                clock.SetCurrentTime(0, 0, 0f);
                progressBar.SetValueDirectly(1f);
                currentTimeText.text = ZERO_HOURS;
            }

            PlayButton.interactable = true;
            PauseButton.interactable = false;
            StopButton.interactable = AudioListener.pause ? true : false;
        }

        checkClipsCooldownIsOff = true;
    }
    
    private void OnClockSecondsChange(double seconds)
    {
        if(clipsArePlaying)
        {
            progressBar.SetValueReverseOne((float)seconds / totalPlaylistTime);
            currentTimeText.text = timeTextFromCycleTime((float)seconds);
        }
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
            || !PlayButton
            || !PauseButton
            || !StopButton
            )
        {            
            Debug.LogWarning("Player: one or more components are not assigned.");
            return;
        }

        PC = FindObjectOfType<ProgramController>();
        course = FindObjectOfType<Course>();

        clock.OnSecondsChange.AddListener(OnClockSecondsChange);

        validated = true;
    }
}
