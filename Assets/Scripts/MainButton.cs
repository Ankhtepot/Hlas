using Assets.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

public class MainButton : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] TextMeshProUGUI purchaseText;
    [SerializeField] Image purchaseImage;
    [SerializeField] Animator animator;
    [Header("Observed fields")]
    public CourseConfiguration Configuration;
    [SerializeField] bool validated = false;

    [HideInInspector] public OnClickEvent OnClick;
#pragma warning restore 649, 414

    [System.Serializable] public class OnClickEvent : UnityEvent<CourseConfiguration> { };

    void Start()
    {
        initialization();
    }

    void Update()
    {

    }

    private void OnClickListener()
    {
        if (validated)
        {
            OnClick.Invoke(Configuration);
        }
    }

    private void initialization()
    {
        button.image.color = Configuration.ButtonBackgroundColor;
        button.onClick.AddListener(OnClickListener);

        mainText.text = Configuration.CourseName;
        mainText.color = Configuration.ButtonTextColor;

        purchaseText.color = Configuration.ButtonTextColor;
        SetPurchasedText(Configuration.Purchased);

        purchaseImage.color = Configuration.ButtonBackgroundColor;

        validated = true;
    }

    public void SetPurchasedText(bool purchased)
    {
        purchaseText.text = purchased ? strings.ZAKOUPENO : strings.KOUPIT;
    }

    public void Shown(bool shown)
    {
        animator.SetBool(triggers.HIDE, !shown);
    }
}
