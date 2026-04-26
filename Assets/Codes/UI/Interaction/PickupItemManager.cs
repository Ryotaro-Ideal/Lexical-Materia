using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI側（キャンバス等）にアタッチして、プレイヤーの周囲にあるインタラクト対象を監視・表示するマネージャー。
/// </summary>
public class PickupItemManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TooltipUI tooltipUI;      // インスペクターで直接指定（2つあってもこれで確実）
    private Transform playerTransform;                // プレイヤーは自動検索

    [Header("Layer Settings")]
    public LayerMask interactLayer = ~0;              // 対象レイヤー（IInteractableを持つもの）

    [Header("Interaction Settings")]
    public float pickupDistance = 3f;
    public bool useCone = false;                      // 正面のみに限定するか
    [Range(10f, 180f)]
    public float coneHalfAngle = 60f;                 // 扇形の半角
    public float distanceHysteresis = 0.25f;          // ちらつき防止
    public float hoverShowDistanceMultiplier = 1f;

    private Camera mainCamera;
    private InputHandler input;
    private IInteractable currentHover;
    private IInteractable lastShownHover;
    private bool isTooltipShown = false;

    // Collider と IInteractable をペアで保持する（クラスレベルで定義）
    private struct Candidate
    {
        public IInteractable item;
        public Vector3 closestPoint; // コライダー上のプレイヤーへの最近接点
        public float distance;       // プレイヤーから最近接点までの距離
    }

    void Awake()
    {
        mainCamera = Camera.main;

        // シーン内のメインカメラを取得（再確認）
        if (mainCamera == null) mainCamera = Camera.main;

        // プレイヤーを探す（タグまたはコンポーネント）
        FindPlayer();

        input = FindFirstObjectByType<InputHandler>();
    }

    void Update()
    {
        // プレイヤーがいない場合は探し続ける（シーン読み込み直後など）
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        ShowUI();
    }

    private void FindPlayer()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            input = player.GetComponent<InputHandler>();
        }
    }

    private bool ShowUI()
    {
        if (tooltipUI == null) return false;

        // UI上にマウスがあるときは無効化
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            tooltipUI.Hide();
            return false;
        }

        // 周囲のコライダーを取得
        Collider[] cols = Physics.OverlapSphere(playerTransform.position, pickupDistance, interactLayer.value);
        if (cols == null || cols.Length == 0)
        {
            ClearHover();
            return false;
        }

        List<Candidate> candidates = new List<Candidate>(cols.Length);
        Vector3 forward = playerTransform.forward;

        foreach (var col in cols)
        {
            var item = col.GetComponent<IInteractable>();
            if (item == null) continue;

            // pivotではなくコライダー表面の最近接点を使う
            // → オブジェクトが大きくてもコライダーが範囲内なら正しく検知できる
            Vector3 closest = col.ClosestPoint(playerTransform.position);
            float dist = Vector3.Distance(playerTransform.position, closest);

            if (useCone)
            {
                Vector3 toItem = closest - playerTransform.position;
                Vector3 toItemH = new Vector3(toItem.x, 0f, toItem.z);
                Vector3 forwardH = new Vector3(forward.x, 0f, forward.z);
                if (toItemH.sqrMagnitude > 0.0001f)
                {
                    float angle = Vector3.Angle(forwardH, toItemH);
                    if (angle > coneHalfAngle) continue;
                }
            }
            candidates.Add(new Candidate { item = item, closestPoint = closest, distance = dist });
        }

        if (candidates.Count == 0)
        {
            ClearHover();
            return false;
        }

        // 最近接点を基準にソート（角度優先 → 距離でタイブレーク）
        candidates.Sort((a, b) =>
        {
            Vector3 va = a.closestPoint - playerTransform.position;
            Vector3 vb = b.closestPoint - playerTransform.position;
            float angleA = Vector3.Angle(new Vector3(forward.x, 0, forward.z), new Vector3(va.x, 0, va.z));
            float angleB = Vector3.Angle(new Vector3(forward.x, 0, forward.z), new Vector3(vb.x, 0, vb.z));
            if (Mathf.Abs(angleA - angleB) > 0.01f) return angleA.CompareTo(angleB);
            return a.distance.CompareTo(b.distance);
        });

        Candidate best = candidates[0];
        IInteractable target = best.item;

        // 距離チェック（コライダー最近接点ベース）
        float baseThreshold = pickupDistance * hoverShowDistanceMultiplier;
        float showThreshold = baseThreshold - distanceHysteresis;
        float hideThreshold = baseThreshold + distanceHysteresis;
        float distToTarget = best.distance;

        bool shouldShow = isTooltipShown && lastShownHover == target ? distToTarget <= hideThreshold : distToTarget <= showThreshold;

        if (shouldShow)
        {
            currentHover = target;
            lastShownHover = target;
            isTooltipShown = true;

            // ツールチップ表示位置はpivotを使用（UIの見た目として自然なため）
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.GetPos());
            if (screenPos.z <= 0f)
            {
                tooltipUI.Hide();
            }
            else
            {
                tooltipUI.Show(target.GetName(), screenPos);
            }
        }
        else
        {
            ClearHover();
            return false;
        }
        return true;
    }

    public IInteractable GetTargetItem() => currentHover;

    public void ClearHover()
    {
        currentHover = null;
        lastShownHover = null;
        isTooltipShown = false;
        if (tooltipUI != null) tooltipUI.Hide();
    }
}
