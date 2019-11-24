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
    [SerializeField] AudioSource player;
    [Header("Observed fields")]
    [SerializeField] float startPlayAt = 0f;
    [SerializeField] bool clipsArePlaying = false;
    [SerializeField] List<AudioClip> audioClipList;
    [SerializeField] CourseConfiguration configuration;
    [SerializeField] float totalPlaylistTime;
    [SerializeField] ProgramController PC;
    [SerializeField] Course course;
    [SerializeField] bool validated = false;
    private Dictionary<int, ClipDescription> clipMap = new Dictionary<int, ClipDescription>();

    public const string ZERO_HOURS = "00:00";
    public int currentlyPlayingSongNr = 0;
#pragma warning restore 649, 414

    void Start()
    {
        initilize();
    }

    void Update()
    {
        managePlaySequence();
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

    public void SetScreen(CourseConfiguration configuration, Playlist playlist)
    {
        if (!validated) return;

        this.configuration = configuration;
        totalPlaylistTime = playlist.GetTotalDuration();
        fillAudioClipList(playlist);
        fillClipMap(audioClipList);

        clock.SetTimeFramework(0, 0, totalPlaylistTime, 0, 0, 0d, false);

        courseText.text = configuration.CourseName;
        playingInfoText.text = playlist.playlistName;
        backToCourseButton.image.color = configuration.ButtonBackgroundColor;
        backToHomeButton.image.color = configuration.ButtonBackgroundColor;
        currentTimeText.text = ZERO_HOURS;
        totalTimeText.text = timeTextFromCycleTime(totalPlaylistTime);

        animator.SetBool(triggers.SHOW, true);
    }



    public void StartPlaying()
    {
        StartPlaying(startPlayAt);
    }

    public void StartPlaying(float startTime)
    {
        if (audioClipList.Count == 0)
        {
            Debug.LogWarning("Player: there are no clips to play.");
            return;
        }

        clock.SetActive(false);
        clipsArePlaying = false;

        PlayButton.interactable = false;
        PauseButton.interactable = true;
        StopButton.interactable = true;

        if (AudioListener.pause == false)
        {
            setStartPosition(startTime);

            clock.SetCurrentTime(0, 0, startPlayAt);

            player.clip = audioClipList[currentlyPlayingSongNr];
            player.time = startPlayAt;
            clock.SetCurrentTime(0, 0, startTime);
            player.Play();
        }
        else
        {
            AudioListener.pause = false;
        }

        clock.SetActive(true);
        startPlayAt = 0f;
        clipsArePlaying = true;
    }

    public void PausePlaying()
    {
        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = true;

        clock.SetActive(false);
        AudioListener.pause = true;
    }

    public void StopPlaying()
    {
        PlayButton.interactable = true;
        PauseButton.interactable = false;
        StopButton.interactable = false;

        player.Stop();
        currentlyPlayingSongNr = 0;

        clipsArePlaying = false;

        clock.SetActive(false);

        slider.onValueChanged.RemoveListener(sliderOnChangeListener);
        slider.value = 0f;
        slider.onValueChanged.AddListener(sliderOnChangeListener);

        progressBar.SetValueDirectly(1f);
        AudioListener.pause = false;
        StopAllCoroutines();
        currentTimeText.text = ZERO_HOURS;
    }

    private void clear()
    {
        StopPlaying();
        animator.SetBool(triggers.SHOW, false);
        audioClipList.Clear();
        clipMap.Clear();
    }

    private void managePlaySequence()
    {
        if (clipsArePlaying && !AudioListener.pause && !player.isPlaying)
        {
            if (currentlyPlayingSongNr == audioClipList.Count - 1)
            {
                StopPlaying();
            }
            else
            {
                currentlyPlayingSongNr++;
                player.clip = audioClipList[currentlyPlayingSongNr];
                player.time = 0f;
                player.Play();
            }
        }
    }

    private void fillAudioClipList(Playlist playlist)
    {
        playlist.blocks.ForEach(block =>
        {
            if (block.GetClip())
            {
                audioClipList.Add(block.GetClip());
            }
        });
    }

    private void fillClipMap(List<AudioClip> clips)
    {
        float startTime = 0;

        for (int i = 0; i < clips.Count; i++)
        {
            clipMap.Add(i, new ClipDescription(startTime, startTime + clips[i].length));
            startTime += clips[i].length;
        }
    }

    private string timeTextFromCycleTime(float cycleTime)
    {
        int minutes = (int)(cycleTime / 60f);
        int seconds = (int)(cycleTime - (minutes * 60f));
        return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }

    private void setStartPosition(float startT)
    {
        float durationSoFar = 0;

        for (int i = 0; i < clipMap.Count; i++)
        {
            if (startT >= clipMap[i].startTime && startT <= clipMap[i].endTime)
            {
                currentlyPlayingSongNr = i;
                startPlayAt = startT - durationSoFar;
                break;
            }
            else
            {
                durationSoFar += clipMap[i].duration;
            }
        }
    }

    private void OnClockSecondsChange(double seconds)
    {
        if (clipsArePlaying)
        {
            progressBar.SetValueReverseOne((float)seconds / totalPlaylistTime);
            currentTimeText.text = timeTextFromCycleTime((float)seconds);
        }
    }

    private void sliderOnChangeListener(float value)
    {
        float oneProc = totalPlaylistTime / 100f;
        StartPlaying(value * 100 * oneProc);
    }

    private void initilize()
    {
        if (
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

        slider.onValueChanged.AddListener(sliderOnChangeListener);

        validated = true;
    }

    private class ClipDescription
    {
        public float startTime = 0f;
        public float endTime = 0f;
        public float duration = 0f;

        public ClipDescription(float startTime, float endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.duration = endTime - startTime;
        }
    }
}


