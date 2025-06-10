using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class LightOff : MonoBehaviour
{
    private PhotonView photonView;
    public GameObject lampRoot;              // Lamp GameObject를 넣는 슬롯
    // 제어할 Light 컴포넌트
    public Light[] allLight;

    public Renderer[] lampRenderers; // Emission 제어 대상 (램프 메쉬들)

    // 몬스터 감지 범위
    public float detectionRadius = 5f;

    // 감지 대상 태그 (기본값: "Monster")
    public string monster = "Monster";

    // 전등 상태 추적
    public bool isLightOn = true;

    public float interactionRadius = 2f;     // 플레이어 상호작용 범위
    public string playerTag = "Player";      // 플레이어 태그
    public KeyCode interactionKey = KeyCode.F; // 상호작용 키

    private bool isPlayerInRange = false;    // 플레이어가 상호작용 범위 내에 있는지

    void Start()
    {
        // allLight가 비어 있으면 자동으로 자식에서 Light 컴포넌트 찾아서 할당
        if ((allLight == null || allLight.Length == 0) && lampRoot != null)
        {
            allLight = lampRoot.GetComponentsInChildren<Light>();
        }
    }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (lampRenderers == null || lampRenderers.Length == 0)
        {
            lampRenderers = lampRoot.GetComponentsInChildren<Renderer>();
        }
    }

    void Update()
    {
        // 전등이 꺼져있고 플레이어가 상호작용 범위 내에 있을 때
        if (!isLightOn && isPlayerInRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                // TurnOnLight();
                photonView.RPC("TurnOnLight", RpcTarget.All);
            }
        }

        // 전등이 켜져있을 때만 몬스터 감지
        if (!isLightOn) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(monster))
            {
                Debug.Log("몬스터 감지됨, 전등 OFF");
                photonView.RPC("TurnOffLight", RpcTarget.All);
                break;
            }
        }
    }

    [PunRPC]
    public void TurnOffLight()
    {
        foreach (var light in allLight)
        {
            if (light != null) light.enabled = false;
        }

        TurnOffEmission(); // Emission 꺼짐

        isLightOn = false;
    }

    [PunRPC]
    public void TurnOnLight()
    {
        foreach (var light in allLight)
        {
            if (light != null) light.enabled = true;
        }

        TurnOnEmission(); // Emission 켜짐

        isLightOn = true;
    }

    // 플레이어가 상호작용 범위에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            if (!isLightOn)
            {
                Debug.Log("[LightOff] 전등을 켤 수 있습니다. E키를 눌러주세요.");
            }
        }
    }

    // 플레이어가 상호작용 범위를 벗어났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
        }
    }

    // 디버깅용 Gizmo
    void OnDrawGizmosSelected()
    {
        // 몬스터 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 플레이어 상호작용 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public void TurnOffEmission()
    {
        foreach (Renderer r in lampRenderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.IsKeywordEnabled("_EMISSION"))
                {
                    mat.DisableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", Color.black);
                    DynamicGI.SetEmissive(r, Color.black);
                }
            }
        }
    }

    public void TurnOnEmission()
    {
        foreach (Renderer r in lampRenderers)
        {
            Material[] mats = r.materials; // 인스턴스 복사본 확보

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].EnableKeyword("_EMISSION");
                mats[i].SetColor("_EmissionColor", Color.white * 1.5f); // 밝기 조절 가능
                DynamicGI.SetEmissive(r, Color.white * 1.5f);
            }

            r.materials = mats; // 변경 적용 다시 할당
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if ((allLight == null || allLight.Length == 0) && lampRoot != null)
        {
            allLight = lampRoot.GetComponentsInChildren<Light>();
            Debug.Log($"{name} OnValidate: 자동으로 Light {allLight.Length}개 할당");
        }
    }
#endif
}
