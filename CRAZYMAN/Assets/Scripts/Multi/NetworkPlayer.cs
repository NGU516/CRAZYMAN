// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;

// [RequireComponent(typeof(PhotonView))]
// public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable
// {
//     private new PhotonView photonView;
//     private Control playerControl;
//     private FirstPerson firstPerson;
//     private PlayerMove playerMove;
//     private StaminaSystem staminaSystem;
//     private MentalGauge mentalGauge;
//     private Animator animator;

//     private Vector3 networkPosition;
//     private Quaternion networkRotation;
//     private float networkStamina;
//     private float networkMentalGauge;
//     private bool networkIsMoving;
//     private bool networkCanRun;
//     private bool networkCanCrouch;

//     private void Awake()
//     {
//         photonView = GetComponent<PhotonView>();
//         playerControl = GetComponent<Control>();
//         firstPerson = GetComponent<FirstPerson>();
//         playerMove = GetComponent<PlayerMove>();
//         staminaSystem = GetComponent<StaminaSystem>();
//         mentalGauge = GetComponent<MentalGauge>();
//         animator = GetComponent<Animator>();

//         // 로컬 플레이어가 아닌 경우 컨트롤 비활성화
//         if (!photonView.IsMine)
//         {
//             if (playerControl != null) playerControl.enabled = false;
//             if (firstPerson != null) firstPerson.enabled = false;
//             if (playerMove != null) playerMove.enabled = false;

//             // 원격 플레이어의 카메라 비활성화
//             Camera playerCamera = GetComponentInChildren<Camera>();
//             if (playerCamera != null)
//             {
//                 playerCamera.gameObject.SetActive(false);
//             }
//         }
//     }

//     private void Start()
//     {
//         if (photonView.IsMine)
//         {
//             networkPosition = transform.position;
//             networkRotation = transform.rotation;
//             if (staminaSystem != null) networkStamina = staminaSystem.currentStamina;
//             if (mentalGauge != null) networkMentalGauge = mentalGauge.currentMental;
//         }
//     }

//     private void Update()
//     {
//         if (!photonView.IsMine)
//         {
//             // 원격 플레이어 위치 보간
//             transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
//             transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);

//             // 원격 플레이어 애니메이션 동기화
//             if (animator != null)
//             {
//                 animator.SetBool("isMoving", networkIsMoving);
//                 animator.SetBool("canRun", networkCanRun);
//                 animator.SetBool("canCrouch", networkCanCrouch);
//             }
//         }
//     }

//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         if (stream.IsWriting)
//         {
//             // 데이터 전송
//             stream.SendNext(transform.position);
//             stream.SendNext(transform.rotation);
//             // 스테미나, 정신력 변수 public으로 변경 필요
//             if (staminaSystem != null) stream.SendNext(staminaSystem.currentStamina);
//             if (mentalGauge != null) stream.SendNext(mentalGauge.currentMental);
//             if (playerControl != null)
//             {
//                 stream.SendNext(playerControl.v > 0.01f || playerControl.v < -0.01f);
//                 stream.SendNext(playerControl.canRun);
//                 stream.SendNext(playerControl.canCrouch);
//             }
//         }
//         else
//         {
//             // 데이터 수신
//             networkPosition = (Vector3)stream.ReceiveNext();
//             networkRotation = (Quaternion)stream.ReceiveNext();
//             if (staminaSystem != null) networkStamina = (float)stream.ReceiveNext();
//             if (mentalGauge != null) networkMentalGauge = (float)stream.ReceiveNext();
//             if (playerControl != null)
//             {
//                 networkIsMoving = (bool)stream.ReceiveNext();
//                 networkCanRun = (bool)stream.ReceiveNext();
//                 networkCanCrouch = (bool)stream.ReceiveNext();
//             }
//         }
//     }

//     [PunRPC]
//     public void TakeDamage(float damage)
//     {
//         if (photonView.IsMine)
//         {
//             if (mentalGauge != null)
//             {
//                 mentalGauge.TriggerDeathByEnemy();
//             }
//         }
//     }

//     [PunRPC]
//     public void UseStamina(float amount)
//     {
//         if (photonView.IsMine && staminaSystem != null)
//         {
//             staminaSystem.StartDraining();
//         }
//     }

//     // 플레이어 사망 처리
//     public void OnPlayerDeath()
//     {
//         if (photonView.IsMine)
//         {
//             // 사망 시 재스폰 요청
//             // NetworkManager.Instance.RequestRespawn();
//         }
//     }
// } 