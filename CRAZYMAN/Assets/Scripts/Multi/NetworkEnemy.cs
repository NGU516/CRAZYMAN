using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

[RequireComponent(typeof(PhotonView))]
public class NetworkEnemy : MonoBehaviourPunCallbacks, IPunObservable
{
    // 네트워크 동기화를 위한 괴인 상태 enum
    private enum NetworkState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    private new PhotonView photonView;
    private EnemyAI enemyAI;
    private EnemyPatrol enemyPatrol;
    private EnemyChase enemyChase;
    private EnemyAttack enemyAttack;
    private Animator enemyAnimator;
    private NavMeshAgent agent;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float networkHealth = 100f;
    private NetworkState networkState;
    private int networkAttackIndex = -1;
    private bool networkIsAttacking = false;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        enemyAI = GetComponent<EnemyAI>();
        enemyPatrol = GetComponent<EnemyPatrol>();
        enemyChase = GetComponent<EnemyChase>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 마스터 클라이언트가 아닌 경우 AI 제어 비활성화
        if (!PhotonNetwork.IsMasterClient)
        {
            if (enemyAI != null) enemyAI.enabled = false;
            if (enemyPatrol != null) enemyPatrol.enabled = false;
            if (enemyChase != null) enemyChase.enabled = false;
            if (enemyAttack != null) enemyAttack.enabled = false;
            if (agent != null) agent.enabled = false;
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
            networkState = NetworkState.Idle;

            GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
            if (localPlayer != null)
            {
                photonView.RPC("SetTarget", RpcTarget.AllBuffered, localPlayer.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // 원격 적 위치 보간
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);

            // 원격 적 애니메이션 동기화
            if (enemyAnimator != null)
            {
                enemyAnimator.SetInteger("EnemyState", (int)networkState);
                if (networkIsAttacking)
                {
                    enemyAnimator.SetInteger("AttackIndex", networkAttackIndex);
                    enemyAnimator.SetTrigger("Attack");
                    networkIsAttacking = false;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(networkHealth);
            stream.SendNext((int)networkState);
        }
        else
        {
            // 데이터 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkHealth = (float)stream.ReceiveNext();
            networkState = (NetworkState)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (enemyAI != null)
            {
                // EnemyAI의 TakeDamage 메서드 호출
                SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                
                // 공격자 추적
                PhotonView attackerView = PhotonView.Find(attackerViewID);
                if (attackerView != null)
                {
                    // EnemyAI의 SetTarget 메서드 호출
                    SendMessage("SetTarget", attackerView.transform, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    [PunRPC]
    public void SetState(int newState)
    {
        // 원격 클라이언트가 받은 상태를 저장만 함 (AI 비활성화 상태에서)
        if (!PhotonNetwork.IsMasterClient)
        {
            networkState = (NetworkState)newState;
        }
    }

    [PunRPC]
    public void SetTarget(int targetViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView != null && enemyAI != null)
            {
                // EnemyAI의 SetTarget 메서드 호출
                SendMessage("SetTarget", targetView.transform, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    [PunRPC]
    public void TriggerAttack(int attackIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            networkAttackIndex = attackIndex;
            networkIsAttacking = true;
            photonView.RPC("SyncAttack", RpcTarget.Others, attackIndex);
        }
    }

    [PunRPC]
    private void SyncAttack(int attackIndex)
    {
        networkAttackIndex = attackIndex;
        networkIsAttacking = true;
    }
} 