using UnityEngine;
using UnityEngine.Events;

public class UIActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private TextGimmickUIController uiController;

    [SerializeField] private string interactName = "調べる";

    [Header("ギミック設定")]
    [SerializeField] private string correctWord = "あける";
    [SerializeField] private UnityEvent onCorrectAction;

    public void Interact()
    {
        if (uiController != null)
        {
            uiController.OpenGimmickUI(correctWord, onCorrectAction);
        }
        else
        {
            Debug.LogWarning("UIController がアタッチされていません！");
        }
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public string GetName()
    {
        return interactName;
    }
}
