using System;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene { MainMenu, Default, Loading}

    private static Action OnLoaderCallback;

    public static void Load(Scene pScene)
    {
        OnLoaderCallback = () => { SceneManager.LoadScene(pScene.ToString());};

        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        if (OnLoaderCallback != null)
        {
            OnLoaderCallback();
            OnLoaderCallback = null;
        }
    }
}
