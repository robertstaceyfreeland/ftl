using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSystemOptions : MonoBehaviour
{
    public GameObject _Menu;
    public bool _IsGamePaused = false;

    

    void Start()
    {
        GameHandler.Instance.OnGamePaused += GameHandler_OnGamePaused;
    }

    private void GameHandler_OnGamePaused(object sender, EventArgs e)
    {
        ToggleGamePaused();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        Achievements.Instance._ReloadedGame = true;
        StartCoroutine(WaitForDing(Loader.Scene.MainMenu));
    }

    private IEnumerator WaitForDing(Loader.Scene pScene)
    {
        yield return new WaitForSeconds(.6f);

        Loader.Load(pScene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleGamePaused()
    {
        if (_IsGamePaused)
        {
            Time.timeScale = 1;
            _IsGamePaused = false;
            _Menu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            _IsGamePaused = true;
            _Menu.SetActive(true);
        }
    }

    
}
