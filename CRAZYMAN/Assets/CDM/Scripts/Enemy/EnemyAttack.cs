// EnemyAttack.cs
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 2.5f;
    public float fieldOfView = 120f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 leftRay = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 rightRay = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRay * attackRange);
        Gizmos.DrawRay(transform.position, rightRay * attackRange);
    }
}
