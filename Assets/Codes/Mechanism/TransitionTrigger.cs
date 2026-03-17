using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName; // 遷移先のシーン名
    [SerializeField] private LayerMask playerLayer; // プレイヤーのレイヤー

    // プレイヤーが触れたら
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("TransitionTrigger: Enter ");
        // プレイヤーかどうか判定（TagがPlayerの設定が必要）
        if (playerLayer == (playerLayer | (1 << collision.gameObject.layer)))
        {

            // TransitionManager を通してシーン切り替え
            if (TransitionManager.Instance != null)
            {
                TransitionManager.Instance.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("TransitionManager がシーン内に存在しません。");
            }
        }
    }
}
