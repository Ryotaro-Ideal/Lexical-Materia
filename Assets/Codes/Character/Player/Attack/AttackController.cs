using UnityEngine;

public class AttackController : MonoBehaviour
{
    private AttackBase currentAttack;
    private AnimationController animController;
    [SerializeField] private AttackBase[] attacks;
    void Awake()
    {
        animController = GetComponent<AnimationController>();
        if (attacks.Length == 0)
        {
            Debug.Log("AttackController: No attacks assigned");
            return;
        }
        currentAttack = attacks[0];

    }
    void Update()
    {

    }
    public void Attack()
    {
        Debug.Log("AttackController: Attack called");
        currentAttack?.ExcecuteAttack(animController);
    }
    public void SetAttack(AttackBase newAttack)
    {
        currentAttack = newAttack;
    }
}