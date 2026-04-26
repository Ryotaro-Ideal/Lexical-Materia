using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] targetObjects;

    public void Activate()
    {
        foreach (var obj in targetObjects)
        {
            if (obj != null) obj.SetActive(true);
        }
    }
}
