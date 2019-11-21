using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fireball Games * * * PetrZavodny.com

public class LogoButton : MonoBehaviour
{
#pragma warning disable 649, 414
    [SerializeField] bool expanded = false;
    [SerializeField] Animator animator;
#pragma warning restore 649, 414

    public void OnButtonClick()
    {
        if( expanded)
        {
            expanded = false;
            animator.SetBool(triggers.EXPAND, false);
        }
        else 
        {
            expanded = true;
            animator.SetBool(triggers.EXPAND, true);
        }
         
    }
}
