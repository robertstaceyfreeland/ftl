using System;
using UnityEngine;
using UnityEngine.UI;

public class JourneyDisplay : MonoBehaviour
{
    GameHandler _GameHandler;
    JourneyStep _CurrentJourneyStep;
    
    void Start()
    {
        //Image MyImage;
        //var myObject = GameObject.Find("Jump_01");

        //MyImage = myObject.GetComponent<Image>();
        //MyImage.color = Color.red;

        _GameHandler = GameObject.FindObjectOfType<GameHandler>();

        _GameHandler.OnJourneyStepChanged += GameHandler_OnJourneyStepChanged;
    }

    private void GameHandler_OnJourneyStepChanged(object sender, EventArgs e)
    {
        _CurrentJourneyStep =  _GameHandler.GetJourneyStep();

        Image MyImage;
        var myObject = GameObject.Find(_CurrentJourneyStep.JourneyDisplayName);

        //Debug.Log("Name: " + myObject.name);
        MyImage = myObject.GetComponent<Image>();
        MyImage.color = Color.red;

    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
