using UnityEngine;


public class ObjectDeactivator : MonoBehaviour
{
    [SerializeField] private GameObject[] targetObject;


    public void Deactivate()
    {
        if (targetObject != null)
        {
            foreach (var t in targetObject)
            {
                t.SetActive(false);
            }
            Debug.Log($"{targetObject[0].name} を非アクティブにしました。");
        }
        else
        {
            // 自分自身を消すデフォルト動作
            gameObject.SetActive(false);
        }
    }
}
