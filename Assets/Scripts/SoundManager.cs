using UnityEngine.Audio;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } //Singleton;
    

    public AudioMixerGroup _AudioMixerGroup_Music;
    public AudioMixerGroup _AudioMixerGroup_Sfx;
    public AudioMixerGroup _AudioMixerGroup_Warp;
    public AudioMixerGroup _AudioMixerGroup_UI;
    public AudioMixerGroup _AudioMixerGroup_Player;
    public AudioMixerGroup _AudioMixerGroup_Enemy;

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

    public enum Sound 
    { 
        Human_LaserPistol, Human_LaserRifle, Human_Flamthrower, Human_Fist,
        Human_Movement, Alien_Movement, Robot_Movement,
        Alien_Laser, Robot_Laser,
        Alien_Fist, Robot_Fist,
        Human_Grunt, Alien_Grunt, Robot_Grunt,
        Splat,
        Alarm_EnemyShip, Alarm_Intruder, Alarm_Helm,
        AlienScream, AlienDeath,
        Medical,Acknowledgement, Warp
    }

    public enum AudioMixerGroupName { Music, Sfx, Warp, UI, Player, Enemy}

    public void PlaySound(Sound pSound, Vector3 pPosition, AudioMixerGroupName pMixerGroup,float pPitch = 1, float pMin = 0, float pMax = 500)
    {
        GameObject _SoundGameObject = new GameObject("Sound");
        _SoundGameObject.transform.position = pPosition;

        AudioSource _AudioSource = _SoundGameObject.AddComponent<AudioSource>();
        
        _AudioSource.clip = GetAudioClip(pSound);
        _AudioSource.spatialBlend = .75f;
        _AudioSource.pitch = pPitch;
        _AudioSource.rolloffMode = AudioRolloffMode.Linear;
        _AudioSource.maxDistance = pMin;
        _AudioSource.minDistance = pMax;
        _AudioSource.dopplerLevel = 0;
        _AudioSource.outputAudioMixerGroup = GetAudioMixerGroup(pMixerGroup);
        _AudioSource.Play();

        Destroy(_SoundGameObject, _AudioSource.clip.length);
    }

    private AudioClip GetAudioClip(Sound pSound)
    {
        foreach (MyAssets.SoundAudioClip soundAudioClip in MyAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == pSound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound Clip Not Found!");
        return null;
    }

    private AudioMixerGroup GetAudioMixerGroup(AudioMixerGroupName pMixerGroupName)
    {
        switch (pMixerGroupName)
        {
            case AudioMixerGroupName.Music:
                return _AudioMixerGroup_Music;
            case AudioMixerGroupName.Sfx:
                return _AudioMixerGroup_Sfx;
            case AudioMixerGroupName.Warp:
                return _AudioMixerGroup_Warp;
            case AudioMixerGroupName.UI:
                return _AudioMixerGroup_UI;
            case AudioMixerGroupName.Player:
                return _AudioMixerGroup_Player;
            case AudioMixerGroupName.Enemy:
                return _AudioMixerGroup_Enemy;
            default:
                return _AudioMixerGroup_Music;
        }
    }
}
