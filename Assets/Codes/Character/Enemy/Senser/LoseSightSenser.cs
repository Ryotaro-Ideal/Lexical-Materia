using UnityEngine;

public class LoseSightSenser : MonoBehaviour, ILoseSightSenser
{
    [SerializeField] private float loseDistance = 15f;

    public bool HasLostSight(GameObject target)
    {
        if (target == null) return true;
        return Vector3.Distance(transform.position, target.transform.position) > loseDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseDistance);
    }
}
