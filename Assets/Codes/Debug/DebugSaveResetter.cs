using UnityEngine;

public class DebugSaveResetter : MonoBehaviour
{
    [SerializeField] private KeyCode resetKey = KeyCode.F5;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(resetKey))
        {
            ResetAllSaveData();
        }
#endif
    }

    private void ResetAllSaveData()
    {
        SaveManager.Instance.DeleteSave();

        if (InventorySlotManager.Instance != null)
            InventorySlotManager.Instance.ResetSaveData();

        if (LetterInvManager.Instance != null)
            LetterInvManager.Instance.ResetSaveData();

        Debug.Log("[Debug] セーブデータをリセットしました");
    }
}
