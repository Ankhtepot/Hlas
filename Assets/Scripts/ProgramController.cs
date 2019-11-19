using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Constants.enums;

//Fireball Games * * * PetrZavodny.com

public class ProgramController : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] bool OHPurchased = false;
    [SerializeField] bool DGPurchased = false;
    [SerializeField] bool TZPurchased = false;
    [Header("MainScreen fields")]
    [SerializeField] MainButton mainButtonPrefab;
    [Header("PurchaseScreen fields")]
    [Header("Configurations")]
    [SerializeField] List<CourseConfiguration> Configurations = new List<CourseConfiguration>();
    [SerializeField] GameObject MainButtonParent;
    [Header("Observed fields")]
    [SerializeField] int purchasedCoursesCount = 0;
    [SerializeField] Purchase purchase;
    [SerializeField] List<MainButton> mainButtons = new List<MainButton>();
    [SerializeField] bool validated = false;
    private ProgramController instance = null;
#pragma warning restore 649, 414

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            print("Destroying duplicate Globals");
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        initialize();
    }

    void Update()
    {
        
    }

    private void OnMainButtonClickListener(CourseConfiguration configuration)
    {
        print("received course MainButtonClick with id: " + configuration.CourseName);
        if(configuration.Purchased)
        {
            setCourseScreen();
        }
        else
        {
            setPurchaseScreen(configuration);
        }

    }

    private void createMainButtons()
    {
        if(Configurations.Count > 0)
        {
            for (int i = 0; i < Configurations.Count; i++)
            {
                var newButton = Instantiate(mainButtonPrefab);
                newButton.GetComponent<MainButton>().Configuration = Configurations[i];
                newButton.transform.SetParent(MainButtonParent.transform, false);
                RectTransform rectT = newButton.GetComponent<RectTransform>();
                rectT.anchoredPosition -= new Vector2(0, 244 * i);
                mainButtons.Add(newButton);
                purchasedCoursesCount++;
            }

        }
    }

    private void subscribeToMainButtons()
    {
        MainButton[] mainButtons = FindObjectsOfType<MainButton>();
        if(mainButtons.Length > 0)
        {
            foreach (var mainButton in mainButtons)
            {
                mainButton.OnClick.AddListener(OnMainButtonClickListener);
            }
        }
    }

    public void SetMainScreen()
    {
        if(mainButtons.Count == 0 && Configurations.Count > 0)
        {
            print("Setting main screen");
            createMainButtons();
            subscribeToMainButtons();
            
        }
        else
        {
            showHideMainButtons(true);

            foreach (var button in mainButtons)
            {
                button.SetPurchasedText(button.Configuration.Purchased);
            }
        }
    }

    private void setPurchaseScreen(CourseConfiguration configuration)
    {
        print("setting purchaseScreen");
        showHideMainButtons(false);
        purchase.setScreen(configuration);
    }

    private void setCourseScreen()
    {
        print("setting courseScreen");
    }

    private void showHideMainButtons(bool shown)
    {
        foreach (var button in mainButtons)
        {
            button.Shown(shown);
        }
    }

    private void initialize()
    {
        purchase = GetComponent<Purchase>();
        SetMainScreen();

        validated = true;
    }
}
