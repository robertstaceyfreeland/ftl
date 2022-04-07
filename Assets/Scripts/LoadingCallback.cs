using UnityEngine;
using System.Collections;

public class LoadingCallback : MonoBehaviour
{
    private bool _IsFirstUpdate = true;


    private void Update()
    {
        if (_IsFirstUpdate)
        {
            _IsFirstUpdate = false;
            Loader.LoaderCallback();
        }
    }
}
