using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// アイテムなしで無条件にインタラクト可能なオブジェクト。
/// 例：調べられる看板、話しかけられるNPC、踏むと起動するギミックなど。
/// </summary>
public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionName = "調べる"; // UIに表示する名前
    [SerializeField] private bool isOneShot = false; // 一度だけ実行するか

    public UnityEvent OnInteract; // インタラクトした時の処理

    private bool isExecuted = false; // すでに実行済みか

    public void Interact()
    {
        if (isExecuted && isOneShot) return;

        if (isOneShot) isExecuted = true;

        OnInteract?.Invoke();
    }

    public string GetName()
    {
        return interactionName;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
}
