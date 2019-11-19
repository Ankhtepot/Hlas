using Assets.Scripts.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Fireball Games * * * PetrZavodny.com

public class Purchase : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] TextMeshProUGUI buyCourseText;
    [SerializeField] Button YesButton;
    [SerializeField] Button NoButton;
    [SerializeField] Animator animator;
    [SerializeField] Animator processingOverlayAnimator;
    [SerializeField] ProgramController PC;
    [Header("Observed fields")]
    [SerializeField] bool validated = false;
    [SerializeField] CourseConfiguration configuration;
#pragma warning restore 649, 414

    void Start()
    {
        initialize();
    }

    void Update()
    {
        
    }

    public void setScreen(CourseConfiguration configuration)
    {
        if (!validated) return;

        this.configuration = configuration;
        buyCourseText.text = strings.BUY_COURSE_PREFIX + configuration.CourseName + "<br><br>" + configuration.Prize + "Kč";

        animator.SetBool(triggers.SHOW, true);
    }

    private void BrinfUpMainScreen()
    {
        configuration = null;
        animator.SetBool(triggers.SHOW, false);
        PC.SetMainScreen();
    }

    public void NoClick()
    {
        animator.SetBool(triggers.SHOW, false);

        PC.SetMainScreen();
    }

    public void YesClick()
    {
        StartCoroutine(simulatePurchasing());
    }

    private IEnumerator simulatePurchasing()
    {
        processingOverlayAnimator.SetBool(triggers.SHOW, true);
        configuration.SetPurchased(true);
        print("before yield");
        yield return new WaitForSecondsRealtime(3f);
        print("after yield");
        processingOverlayAnimator.SetBool(triggers.SHOW, false);
        BrinfUpMainScreen();
    }

    private void initialize()
    {
        if(
            !YesButton 
            || !NoButton 
            || !buyCourseText
            || !animator
            || !PC
            || !processingOverlayAnimator)
        {
            Debug.LogWarning("Purchase: some component/s are not assigned.");
            return;
        }

        validated = true;
    }
}
