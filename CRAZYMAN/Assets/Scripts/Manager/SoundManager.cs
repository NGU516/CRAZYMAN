using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager
{
    private AudioSource[] _audioSources = new AudioSource[System.Enum.GetValues(typeof(Define.Sound)).Length];
    private Dictionary<string, AudioClip> _audioClips = new();
    private GameObject _soundRoot = null;

    public void Init()
    {
        if (_soundRoot == null)
        {
            _soundRoot = GameObject.Find("@SoundRoot");

            if (_soundRoot == null)
            {
                _soundRoot = new GameObject { name = "@SoundRoot" };
                Object.DontDestroyOnLoad(_soundRoot);
                string[] soundTypeNames = System.Enum.GetNames(typeof(Define.Sound));

                for (int i = 0; i < soundTypeNames.Length; ++i)
                {
                    GameObject go = new GameObject { name = soundTypeNames[i] };
                    _audioSources[i] = go.AddComponent<AudioSource>();
                    _audioSources[i].playOnAwake = false;
                    go.transform.parent = _soundRoot.transform;
                }

                _audioSources[(int)Define.Sound.Bgm].loop = true;
                _audioSources[(int)Define.Sound.Bgm].playOnAwake = true;
            }
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
            audioSource.Stop();
        _audioClips.Clear();
    }

    public bool Play(Define.Sound type, string path, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        AudioSource audioSource = _audioSources[(int)type];
        if (path.Contains("Audio/") == false)
            path = string.Format("Audio/{0}", path);

        audioSource.volume = volume;

        if (type == Define.Sound.Bgm)
        {
            AudioClip audioClip = Managers.Resource.Load<AudioClip>(path);
            if (audioClip == null)
                return false;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.Play();
            return true;
        }
        else if (type == Define.Sound.Throw || type == Define.Sound.Merge)
        {
            AudioClip audioClip = GetAudioClip(path);
            if (audioClip == null)
                return false;

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        else if (type == Define.Sound.UseItem)
        {
            AudioClip audioClip = GetAudioClip(path);
            if (audioClip == null)
            {
                return false;
            }

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        else if (type == Define.Sound.GetItem)
        {
            AudioClip audioClip = GetAudioClip(path);
            if (audioClip == null)
            {
                return false;
            }

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        return false;
    }

    private AudioClip GetAudioClip(string path)
    {
        AudioClip audioClip = null;
        if (_audioClips.TryGetValue(path, out audioClip))
            return audioClip;

        audioClip = Managers.Resource.Load<AudioClip>(path);
        _audioClips.Add(path, audioClip);
        return audioClip;
    }
}
