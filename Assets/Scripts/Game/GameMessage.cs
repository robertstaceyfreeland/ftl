using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using TMPro;



public class GameMessage : MonoBehaviour
{
    public enum MessageType { StartMessage, BeforeLevel, Warp, InLevel, AfterLevel, GameOver, Idle}

    public MessageType _MessageType;
    public Lean.Localization.LeanLocalizedText _HeaderText;
    public Lean.Localization.LeanLocalizedText _BodyText_01;
    public Lean.Localization.LeanLocalizedText _BodyText_02;

    private void Start()
    {
        Time.timeScale = 0;
    }

    public void NewMessage(string pHeaderText = "", string pBodyText_01 = "", string pBodyText_02 = "", MessageType pMessageType = MessageType.Idle)
    {
        Time.timeScale = 0;

        _MessageType = pMessageType;
        _HeaderText.FallbackText= pHeaderText;
        _BodyText_01.FallbackText = pBodyText_01;
        _BodyText_02.FallbackText = pBodyText_02;

        this.gameObject.SetActive(true);
    }

    public void Done()
    {
        if (_MessageType == MessageType.GameOver)
        {
            SceneManager.LoadScene(0);
            SceneManager.UnloadSceneAsync(1);
            
        }
        else
        {
            Time.timeScale = 1;
            this.gameObject.SetActive(false);
            GameHandler.Instance.MessagReturn(_MessageType);
        }

        
    }
}
