using UnityEngine;


public class ObjectDeactivator : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;


    public void Deactivate()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            Debug.Log($"{targetObject.name} を非アクティブにしました。");
        }
        else
        {
            // 自分自身を消すデフォルト動作
            gameObject.SetActive(false);
        }
    }
}
