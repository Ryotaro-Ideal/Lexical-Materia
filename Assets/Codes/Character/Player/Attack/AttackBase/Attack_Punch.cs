using UnityEngine;
using System.Collections;
public class Attack_Punch : AttackBase
{

    public float pickupDistance = 3f;
    public float coneHalfAngle = 60f;
    [SerializeField] private LayerMask targetLayer;




    //扇形で判定し、IDamageableを持つオブジェクトにダメージを与える
    public override void ExcecuteAttack(Animator animator)
    {
        Debug.Log("Attack_Punch: ExcecuteAttack called");
        animator.SetTrigger("Attack_Punch");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupDistance, targetLayer);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
            // 扇形の内側かどうかをチェック
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if (angleToTarget <= coneHalfAngle)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Debug.Log("Attack_Punch: Damaging " + hitCollider.name);
                    damageable.TakeDamage(1);
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward;
        Quaternion left = Quaternion.AngleAxis(-coneHalfAngle, Vector3.up);
        Quaternion right = Quaternion.AngleAxis(coneHalfAngle, Vector3.up);
        Vector3 a = (left * forward).normalized * pickupDistance;
        Vector3 b = (right * forward).normalized * pickupDistance;
        Gizmos.DrawLine(transform.position, transform.position + a);
        Gizmos.DrawLine(transform.position, transform.position + b);
    }

}