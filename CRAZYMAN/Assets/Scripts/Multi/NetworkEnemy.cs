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
            // agent.enabled = false; // NavMeshAgent 비활성화 주석 처리 (테스트용)
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
            // NavMesh 상태 디버그 로깅 (테스트용)
            // if (agent != null)
            // {
            //     Debug.Log($"[NetworkEnemy] Agent Status - Enabled: {agent.enabled}, " +
            //              $"IsOnNavMesh: {agent.isOnNavMesh}, " +
            //              $"Path Status: {agent.pathStatus}, " +
            //              $"Has Path: {agent.hasPath}");
            // }

            // NavMesh 위에 있는지 확인하고 보정
            if (agent != null && !agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }
            }

            // 원격 적 위치 보간
            float lerpSpeed = agent != null && agent.isOnNavMesh ? 10f : 5f; // NavMesh 상태에 따른 보간 속도 조정
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpSpeed);
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
            // NavMesh 위의 위치만 전송
            NavMeshHit hit;
            Vector3 safePosition = transform.position;
            if (NavMesh.SamplePosition(transform.position, out hit, 0.1f, NavMesh.AllAreas))
            {
                safePosition = hit.position;
            }
            
            stream.SendNext(safePosition);
            stream.SendNext(transform.rotation);
            stream.SendNext(networkHealth);
            stream.SendNext((int)networkState);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkHealth = (float)stream.ReceiveNext();
            networkState = (NetworkState)stream.ReceiveNext();
        }
    }

    // 테스트용 디버그 시각화
    private void OnDrawGizmos()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.position, 0.3f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.3f);
            }
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