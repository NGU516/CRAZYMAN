using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] _audioSources = new AudioSource[System.Enum.GetValues(typeof(Define.Sound)).Length];
    private Dictionary<string, AudioClip> _audioClips = new();
    private GameObject _soundRoot = null;

    // Enum-경로 매핑 딕셔너리
    private Dictionary<Define.Sound, string> _soundPaths = new Dictionary<Define.Sound, string>
    {
        { Define.Sound.PlayerWalk, "player_footstep" }, // 플레이어 걷는 소리
        { Define.Sound.PlayerRun, "player_footstep" }, // 플레이어 달리는 소리
        { Define.Sound.PlayerDie, "player_die" },
        { Define.Sound.EnemyWalk, "enemy_walk_4" }, // 괴인 걷는 소리
        { Define.Sound.EnemyRun, "enemy_breath_2" },
        { Define.Sound.EnemyDie, "test" },
        { Define.Sound.DoorInteract, "door_open" }, // 문 여는 소리
        { Define.Sound.Merge, "test" },
        { Define.Sound.Throw, "test" },
        { Define.Sound.UseItem, "test" },
        { Define.Sound.GetItem, "test" },
        { Define.Sound.Bgm, "horror_bgm" }, // 브금
        { Define.Sound.UseCamera, "camera_1" }, // 카메라 소리
        { Define.Sound.UsePill, "pill_cap" }, // 약 캡슐 소리
        { Define.Sound.swallow, "swallow" }, // 정신력 바닥에 가까워지면 환청 들리게? 
        // 필요에 따라 추가
        { Define.Sound.Flashlight, "flashlight_on_off" }, // 손전등 on/off 소리
    };

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

    // 오버로딩: Enum만 받아서 내부적으로 경로를 찾아서 재생
    public bool Play(Define.Sound type, float volume = 1.0f, float pitch = 1.0f)
    {
        if (!_soundPaths.TryGetValue(type, out string path))
        {
            Debug.LogWarning($"Sound path not found for {type}");
            return false;
        }
        return Play(type, path, volume, pitch);
    }

    // 기존 Play 함수(경로 직접 지정)는 내부적으로만 사용
    public bool Play(Define.Sound type, string path, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        AudioSource audioSource = _audioSources[(int)type];
        if (path.Contains("Audio/") == false)
            path = string.Format("Audio/{0}", path);

        audioSource.volume = volume;

        switch (type)
        {
            case Define.Sound.Bgm:
                return PlayBGM(audioSource, path, pitch);
            
            case Define.Sound.Merge:
            case Define.Sound.Throw:
            case Define.Sound.UseItem:
            case Define.Sound.GetItem:
            case Define.Sound.PlayerWalk:
            case Define.Sound.PlayerRun:
            case Define.Sound.PlayerDie:
            case Define.Sound.EnemyWalk:
            case Define.Sound.EnemyRun:
            case Define.Sound.EnemyDie:
            case Define.Sound.DoorInteract:
            case Define.Sound.Flashlight:
            case Define.Sound.UseCamera:
            case Define.Sound.UsePill:
                return PlaySoundEffect(audioSource, path, pitch);

            default:
                return false;
        }
    }

    private bool PlayBGM(AudioSource audioSource, string path, float pitch)
    {
        AudioClip bgmClip = Managers.Resource.Load<AudioClip>(path);
        if (bgmClip == null)
            return false;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = bgmClip;
        audioSource.pitch = pitch;
        audioSource.Play();
        return true;
    }

    private bool PlaySoundEffect(AudioSource audioSource, string path, float pitch)
    {
        AudioClip effectClip = GetAudioClip(path);
        if (effectClip == null)
            return false;

        // 오디오 소스 초기화
        audioSource.clip = effectClip;
        audioSource.pitch = pitch;
        audioSource.Play();
        return true;
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
