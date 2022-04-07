using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLocalization : MonoBehaviour
{
    private bool _IsOn = true;

    private float NextUpdateTick = 0;

    string CurrentLanguage = "English";
    
    void Start()
    {
        Lean.Localization.LeanLocalization.CurrentLanguage = "English";

        _IsOn = true;
    }

    private void ChangeLanguage()
    {
        switch (CurrentLanguage)
        {
            case "Simplified Chinese":
                CurrentLanguage = "English";
                break;
            case "English":
                CurrentLanguage = "Spanish";
                break;
            case "Spanish":
                CurrentLanguage = "Arabic";
                break;
            case "Arabic":
                CurrentLanguage = "German";
                break;
            case "German":
                CurrentLanguage = "Korean";
                break;
            case "Korean":
                CurrentLanguage = "French";
                break;
            case "French":
                CurrentLanguage = "Russian";
                break;
            case "Russian":
                CurrentLanguage = "Japanese";
                break;
            case "Japanese":
                CurrentLanguage = "Italian";
                break;
            case "Italian":
                CurrentLanguage = "Portuguese";
                break;
            default:
                CurrentLanguage = "Chinese";
                break;
        }

        Lean.Localization.LeanLocalization.CurrentLanguage = CurrentLanguage;
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsOn)
        {
            if (NextUpdateTick < Time.time)
            {
                ChangeLanguage();

                NextUpdateTick = Time.time + 3;
            }
        }
    }
}
