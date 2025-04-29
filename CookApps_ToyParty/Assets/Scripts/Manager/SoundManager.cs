using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = nameof(SoundManager);
                _instance = obj.AddComponent<SoundManager>();
            }
            return _instance;
        }
    }

    public bool isMuteBGM
    {
        get { return _audio.mute == true; }
        set { _audio.mute = value; }
    }

    public bool isMuteSFX
    {
        get { return _sfx.mute == true; }
        set { _sfx.mute = value; }
    }

    AudioSource _audio = null;
    AudioSource _sfx = null;

    public void Init()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        _sfx = gameObject.AddComponent<AudioSource>();
        _audio.volume = 0.6f;
        _audio.loop = true;
        _sfx.volume = 0.6f;
        _sfx.loop = false;
        PlayBGM(eBGM_Type.Main);
    }

    public void PlayBGM(eBGM_Type type)
    {
        string fileName = type.ToDescription();
        if (string.IsNullOrEmpty(fileName) == true)
            return;

        AudioClip clip = Resources.Load<AudioClip>(fileName);
        if (clip == null)
        {
            _audio.Stop();
            return;
        }

        _audio.resource = clip;
        _audio.Play();
    }

    public void PlaySFX(eSFX_Type type)
    {
        string fileName = type.ToDescription();
        if (string.IsNullOrEmpty(fileName) == true)
            return;

        AudioClip clip = Resources.Load<AudioClip>(fileName);
        if(clip == null)
            return;

        _sfx.PlayOneShot(clip);
    }
}
