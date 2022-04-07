using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAssets : MonoBehaviour
{

    // Internal instance reference
    private static MyAssets _i;

    // Instance reference
    public static MyAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<MyAssets>("MyAssets"));
            return _i;
        }
    }

    public Transform myDamagePopup;



    //public AudioClip playerAttack;

    public SoundAudioClip[] soundAudioClipArray;


    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

}
