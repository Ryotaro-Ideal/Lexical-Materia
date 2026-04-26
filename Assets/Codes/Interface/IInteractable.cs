using UnityEngine;
public interface IInteractable
{
    void Interact(); // 「調べられた時」の処理
    Vector3 GetPos();
    string GetName();

}