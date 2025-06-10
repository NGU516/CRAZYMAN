using UnityEngine;

public class UIEnding : UIPopup
{
    [SerializeField] private AudioListener uiAudioListener;
    [SerializeField] private AudioSource bgmSource;

    // 엔딩 팝업에서 보여줄 텍스트, 버튼 등 필요에 따라 추가
    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (uiAudioListener == null)
            uiAudioListener = GetComponentInChildren<AudioListener>(true);

        if (bgmSource != null && !bgmSource.enabled)
            bgmSource.enabled = true;

        EnableThisListenerOnly();

        Debug.Log("@SoundRoot 상태: " + GameObject.Find("@SoundRoot")?.activeInHierarchy);
        Debug.Log("StartBgm AudioSource enabled: " + Managers.SoundManager._audioSources[(int)Define.Sound.StartBgm].enabled);

        Managers.SoundManager.Init();
        Managers.SoundManager.Play(Define.Sound.StartBgm);

        AudioSource src = GameObject.Find("@SoundRoot")?.transform.Find("StartBgm")?.GetComponent<AudioSource>();
        Debug.Log($"🎵 AudioSource 상태: {(src == null ? "NULL" : "존재")}, Enabled: {src?.enabled}, Clip: {src?.clip?.name}, Volume: {src?.volume}");

        Debug.Log("엔딩 BGM 재생");
        return true;
    }

    private void EnableThisListenerOnly()
    {
        AudioListener[] allListeners = GameObject.FindObjectsOfType<AudioListener>(true);
        foreach (var listener in allListeners)
        {
            if (listener == uiAudioListener)
                continue;
            Debug.Log($"🔇 끄는 AudioListener: {listener.gameObject.name}");
            listener.enabled = false;
        }

        if (uiAudioListener != null)
        {
            uiAudioListener.enabled = true;
            Debug.Log("✅ UI AudioListener Enabled");
        }
        else
        {
            Debug.LogWarning("❌ UI AudioListener not found");
        }
    }

}
