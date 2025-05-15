using UnityEngine;

public class AudioEventRX : MonoBehaviour
{
    [SerializeField] private bool isPlayer = true; // 플레이어인지 적인지 구분

    public void PlayFootstepSound()
    {
        if (isPlayer)
        {
            // 플레이어 걷기 소리
            Managers.SoundManager.Play(Define.Sound.PlayerWalk);
            Debug.Log("플레이어 걷기 소리 재생");
        }
        else
        {
            // 괴인 걷기 소리
            Managers.SoundManager.Play(Define.Sound.EnemyWalk);
            Debug.Log("괴인 걷기 소리 재생");
        }
    }

    public void PlayRunSound()
    {
        if (isPlayer)
        {
            Managers.SoundManager.Play(Define.Sound.PlayerRun);
            Debug.Log("플레이어 달리기 소리 재생");
        }
        else
        {
            Managers.SoundManager.Play(Define.Sound.EnemyRun);
            Debug.Log("괴인 달리기 소리 재생");
        }
    }

    public void PlayDieSound()
    {
        Managers.SoundManager.Play(Define.Sound.PlayerDie);
        Debug.Log("플레이어 죽음 소리 재생");
    }

    public void PlayUseItemSound()
    {
        Managers.SoundManager.Play(Define.Sound.UseItem);
        Debug.Log("아이템 사용 소리 재생");
    }

    public void PlayGetItemSound()
    {
        Managers.SoundManager.Play(Define.Sound.GetItem);
        Debug.Log("아이템 획득 소리 재생");
    }

    public void PlayDoorinteractSound()
    {
        Managers.SoundManager.Play(Define.Sound.DoorInteract);
        Debug.Log("문 열기 소리 재생");
    }

    public void PlayMergeSound()
    {
        Managers.SoundManager.Play(Define.Sound.Merge);
        Debug.Log("합치기 소리 재생");
    }

    public void PlayThrowSound()
    {
        Managers.SoundManager.Play(Define.Sound.Throw);
        Debug.Log("던지기 소리 재생");
    }
} 