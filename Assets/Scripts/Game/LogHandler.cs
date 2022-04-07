using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LogHandler : MonoBehaviour
{
    public static LogHandler Instance { get; private set; } //Singleton

    [SerializeField] private Text _MessageLogText;
    [SerializeField] private RectTransform _MessageContent;
    [SerializeField] private ScrollRect _ScrollRect;

    private void Awake()
    {
        #region //Singleton

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void NewLogEntry(string pMessage)
    {
        _MessageLogText.text = _MessageLogText.text  + pMessage + "\n" + ">";
        _ScrollRect.verticalNormalizedPosition = 0;
    }
    private void Update()
    {
        if (!IsMouseOverUi())
        {
            _ScrollRect.verticalNormalizedPosition = 0;
        }
        
    }
    private bool IsMouseOverUi() //Checks to see if mouse is over GUI
    {
        try
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        catch { return false; }
    }


}
