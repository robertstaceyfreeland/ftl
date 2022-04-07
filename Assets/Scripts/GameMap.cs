using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMap : MonoBehaviour
{
    public Image[] _MapButtons;

    void Start()
    {
        foreach (Image image in _MapButtons)
        {
            image.color = Color.gray;
        }
    }

    public void CurrentLevelChanged(int pCurrentLevel)
    {
        if (pCurrentLevel < 0) return;
        
        for (int i = 0; i < pCurrentLevel+1; i++)
        {
            _MapButtons[i].color = Color.green;
        }
    }

    public void ChangeButtonColor(int pButtonIndex, Color pButtonColor)
    {
        _MapButtons[pButtonIndex].color = pButtonColor;
    }
}
