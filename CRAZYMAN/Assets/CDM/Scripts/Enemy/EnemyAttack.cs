// EnemyAttack.cs
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 2.5f;
    public float fieldOfView = 120f;
    public float viewDistance = 7f; // Inspector에서 수정 가능한 시야 범위
    public Transform player;

    void OnDrawGizmosSelected()
    {
        int segments = 30; // 부채꼴을 나눌 세그먼트 수 (많을수록 부드러움)
        float halfFOV = fieldOfView * 0.5f;
        float angleStep = fieldOfView / segments;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // 부채꼴 공격 범위
        Gizmos.color = Color.yellow;
        Vector3 lastPoint = origin + Quaternion.Euler(0, -halfFOV, 0) * forward * attackRange;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 nextPoint = origin + Quaternion.Euler(0, angle, 0) * forward * attackRange;

            // 부채꼴의 테두리 라인을 그림
            Gizmos.DrawLine(lastPoint, nextPoint);

            // 중심에서 퍼지는 선도 함께 그림 (옵션)
            Gizmos.DrawLine(origin, nextPoint);

            lastPoint = nextPoint;
        }

        // 괴인 시야 범위
        Gizmos.color = Color.red;
        Vector3 viewLastPoint = origin + Quaternion.Euler(0, -halfFOV, 0) * forward * viewDistance;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfFOV + angleStep * i;
            Vector3 nextPoint = origin + Quaternion.Euler(0, angle, 0) * forward * viewDistance;

            // 시야 범위의 테두리 라인을 그림
            Gizmos.DrawLine(viewLastPoint, nextPoint);

            // 중심에서 퍼지는 선도 함께 그림
            Gizmos.DrawLine(origin, nextPoint);

            viewLastPoint = nextPoint;
        }
    }

    // 플레이어가 괴인 공격 범위에 있는지 확인
    public bool IsPlayerInAttackCone()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return angleToPlayer <= fieldOfView * 0.5f && distanceToPlayer <= attackRange;
    }
    

    // 플레이어가 괴인 시야에 있는지 확인
    public bool IsPlayerInSight(float viewDistance)
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return angleToPlayer <= fieldOfView * 0.5f && distanceToPlayer <= viewDistance;
    }

}
