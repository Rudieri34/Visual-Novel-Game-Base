using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Volume Managers")]
    [SerializeField] private AudioSource _soundEffectsSource;

    [Header("SFX")]
    [SerializeField] private List<AudioFile> _sfxClips;

    [Header("Ambience")]
    [SerializeField] private GameObject _ambienceAudioPrefab;


    [Header("Loops")]
    [SerializeField] private AudioSource _loopSource;
    [SerializeField] private List<AudioFile> _audioClips;
    public bool IsSequencing;


    private GameObject _player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PlaySFX(string audioName, float pitch = 1, float volume = 1)
    {
        AudioFile sfx = _sfxClips.FirstOrDefault(a => a.audioName == audioName);

        _soundEffectsSource.pitch = pitch;
        _soundEffectsSource.volume = volume;

        if (sfx.audio != null)
            _soundEffectsSource.PlayOneShot(sfx.audio);
        else
            Debug.LogError($"[SoundManager] The audio {audioName} was not found on the SFX clip list");
    }

    public void Play3DAudio(AudioClip clip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayAmbienceAudio(AudioClip clip, float distanceFromPlayer)
    {
        var vector2 = Random.insideUnitCircle.normalized * distanceFromPlayer;
        var prefab = Instantiate(_ambienceAudioPrefab, _player.transform.position + new Vector3(vector2.x, 0, vector2.y), Quaternion.identity);
        var source = prefab.GetComponent<AudioSource>();
        source.clip = clip;
        source.maxDistance = distanceFromPlayer * 2;
        source.Play();
        Destroy(prefab, clip.length + 1);
    }
    public void PlayAudioLoop(string audioName, bool startSequencing = false)
    {
        IsSequencing = startSequencing;
        _loopSource.Stop();
        AudioFile loop = _audioClips.FirstOrDefault(a => a.audioName == audioName);

        if (IsSequencing)
            _loopSource.loop = false;
        else
            _loopSource.loop = true;

        _loopSource.clip = loop.audio;
        _loopSource.Play();
    }
    public void StopAudioLoop()
    {
        IsSequencing = false;
        _loopSource.Stop();
        _loopSource.loop = false;
    }
    public void StopSFX()
    {
        _soundEffectsSource.Stop();
    }


    public void PlayNextSong(AudioClip current)
    {
        int currentIndex = _audioClips.FindIndex(a => a.audio == current);

        AudioFile next = new AudioFile();
        if (currentIndex + 1 >= _audioClips.Count)
            next = _audioClips[0];
        else
            next = _audioClips[currentIndex + 1];

        _loopSource.clip = next.audio;
        _loopSource.Play();
    }

    private void Update()
    {

        if (IsSequencing && _loopSource.isPlaying == false)
        {
            PlayNextSong(_loopSource.clip);
        }
    }
}



[System.Serializable]
public struct AudioFile
{
    public string audioName;
    public AudioClip audio;
}

