using UnityEngine;
using System;

public class SignBoard : MonoBehaviour, IInteractable
{
    public string explanation;
    public Vector3 uiOffset = Vector3.up * 0.6f;
    private string itemName = "未設定";
    public string ItemName { get { return itemName; } }
    private void Awake()
    {
    }
    public virtual void Interact()
    {
        Debug.Log(explanation);
    }
    public Vector3 GetPos()
    {
        return transform.position;
    }
    public string GetName()
    {
        return itemName;
    }
}