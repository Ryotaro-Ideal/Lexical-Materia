using UnityEngine;
public class ChaseSenser : MonoBehaviour, ISenser
{
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private float chaseAngle = 60f;
    [SerializeField] private LayerMask targetLayer;

    public bool TryDetect(out GameObject target)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, chaseDistance, targetLayer);
        foreach (Collider hit in hits)
        {

            Vector3 dir = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle <= chaseAngle * 0.5f)
            {

                target = hit.gameObject;
                return true;
            }
        }
        target = null;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}