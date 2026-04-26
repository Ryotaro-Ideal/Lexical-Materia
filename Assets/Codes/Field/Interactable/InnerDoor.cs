using UnityEngine;

public class InnerDoor : Door
{
    [Header("内鍵設定")]
    [SerializeField] private string lockedMessage = "鍵がかかっている";
    [Tooltip("ドアの正面方向を「内側」として扱う。チェックを外すと逆方向が内側になる。")]
    [SerializeField] private bool forwardIsInside = true;

    private bool isUnlocked = false;

    private bool IsPlayerInside()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Vector3 toPlayer = (player.transform.position - transform.position).normalized;
        Vector3 insideDir = forwardIsInside ? transform.forward : -transform.forward;

        return Vector3.Dot(insideDir, toPlayer) > 0f;
    }

    public override string GetName()
    {
        if (isUnlocked) return base.GetName();
        return IsPlayerInside() ? base.GetName() : lockedMessage;
    }

    public override void Interact()
    {
        if (isUnlocked)
        {
            base.Interact();
            return;
        }

        if (!IsPlayerInside()) return;

        isUnlocked = true;
        base.Interact();
    }
}
