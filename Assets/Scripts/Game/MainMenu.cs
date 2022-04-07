using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Button _ContinueButton;
    public GameObject _OptionMenu;

    private void Start()
    {
        Time.timeScale = 1;

        try
        {
            string _Path = Application.persistentDataPath + "/";

            string[] _FilePaths = Directory.GetFiles(_Path,"*.es3");

            if (_FilePaths.Length > 0)
            {
                _ContinueButton.interactable = true;
            }
            else
            {
                _ContinueButton.interactable = false;
            }
        }

        catch { }

        if(_OptionMenu != null) _OptionMenu.SetActive(false);

        SteamManager _SteamManager = GameObject.FindObjectOfType<SteamManager>();

        Debug.Log("SteamManger Active: " + _SteamManager.isActiveAndEnabled);
    }

    public void PlayGame()
    {
        try
        {
            string _Path = Application.persistentDataPath + "/";

            string[] _FilePaths = Directory.GetFiles(_Path, "*.es3");

            foreach (string filePath in _FilePaths)
            {
                File.Delete(filePath);
            }
        }

        catch { }

        StartCoroutine(WaitForDing());
    }

    private IEnumerator WaitForDing()
    {
        yield return new WaitForSeconds(.7f);

        Loader.Load(Loader.Scene.Default);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        StartCoroutine(WaitForDing());
    }

    public void HighScore()
    {

    }
}
