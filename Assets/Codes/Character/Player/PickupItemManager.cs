using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Transform))]
public class PickupItemManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;      // プレイヤー Transform（未設定なら自動検索）

    public LayerMask interactLayer = ~0;   // 収集対象のレイヤー（例: IInter レイヤー）

    [Header("Pickup Settings")]
    public float pickupDistance = 3f;
    public bool useCone = false;               // 扇形モードを使うか
    [Range(10f, 180f)]
    public float coneHalfAngle = 60f;          // 扇形の半角（degrees）
    public float distanceHysteresis = 0.25f;   // 境界揺れ防止（ヒステリシス）
    public float hoverShowDistanceMultiplier = 1f; // IInter.pickupDistance に掛ける係数


    private TooltipUI tooltipUI;
    private Camera mainCamera;
    private InputHandler input;
    private IInteractable currentHover;
    private IInteractable lastShownHover;
    private bool isTooltipShown = false;

    void Awake()
    {
        mainCamera = Camera.main;
        tooltipUI = FindFirstObjectByType<TooltipUI>();
        var cc = FindFirstObjectByType<CharacterController>();
        if (playerTransform == null && cc != null)
            playerTransform = cc.transform;

        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) playerTransform = p.transform;
        }

        input = FindFirstObjectByType<InputHandler>();
        if (input == null) Debug.LogWarning("PickupManager: No InputHandler found in scene. Attach InputHandler to player.");
    }

    void Update()
    {
        ShowUI();

    }
    private bool ShowUI()
    {
        if (input == null)
        {
            tooltipUI?.Hide();
            return false;
        }

        // UI上にマウスがあるときは拾い/表示を無効化（UIでのクリックと競合させたくないため）
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            tooltipUI?.Hide();
            return false;
        }

        // OverlapSphere で近接オブジェクトを取得
        Collider[] cols = Physics.OverlapSphere(playerTransform.position, pickupDistance, interactLayer.value);
        if (cols == null || cols.Length == 0)
        {
            ClearHover();
            return false;
        }

        // 候補リスト作成（IInter をフィルタ）
        List<IInteractable> candidates = new List<IInteractable>(cols.Length);
        Vector3 forward = playerTransform.forward;
        foreach (var col in cols)
        {
            var item = col.GetComponent<IInteractable>();
            if (item == null) continue;

            if (useCone)
            {
                Vector3 toItem = item.GetPos() - playerTransform.position;
                Vector3 toItemH = new Vector3(toItem.x, 0f, toItem.z);
                Vector3 forwardH = new Vector3(forward.x, 0f, forward.z);
                if (toItemH.sqrMagnitude > 0.0001f)
                {
                    float angle = Vector3.Angle(forwardH, toItemH);
                    if (angle > coneHalfAngle) continue;
                }
            }

            candidates.Add(item);
        }

        if (candidates.Count == 0)
        {
            ClearHover();
            return false;
        }

        // ソート：角度優先（正面に近いもの）→距離優先
        candidates.Sort((a, b) =>
        {
            Vector3 va = a.GetPos() - playerTransform.position;
            Vector3 vb = b.GetPos() - playerTransform.position;
            float angleA = Vector3.Angle(new Vector3(forward.x, 0, forward.z), new Vector3(va.x, 0, va.z));
            float angleB = Vector3.Angle(new Vector3(forward.x, 0, forward.z), new Vector3(vb.x, 0, vb.z));
            if (Mathf.Abs(angleA - angleB) > 0.01f) return angleA.CompareTo(angleB);
            return va.sqrMagnitude.CompareTo(vb.sqrMagnitude);
        });

        IInteractable target = candidates[0];

        // ヒステリシスでちらつきを防ぐ
        float baseThreshold = pickupDistance * hoverShowDistanceMultiplier;
        float showThreshold = baseThreshold - distanceHysteresis;
        float hideThreshold = baseThreshold + distanceHysteresis;
        float distToTarget = Vector3.Distance(playerTransform.position, target.GetPos());

        bool shouldShow;
        if (isTooltipShown && lastShownHover == target)
            shouldShow = distToTarget <= hideThreshold;
        else
            shouldShow = distToTarget <= showThreshold;

        if (shouldShow)
        {
            currentHover = target;
            lastShownHover = target;
            isTooltipShown = true;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.GetPos());
            if (screenPos.z <= 0f)
            {
                tooltipUI?.Hide();
            }
            else
            {
                tooltipUI?.Show(target.GetName(), screenPos);
            }

        }
        else
        {
            ClearHover();
            return false;
        }
        return true;
    }
    public IInteractable GetTargetItem()
    {
        return currentHover;
    }

    public void ClearHover()
    {
        currentHover = null;
        lastShownHover = null;
        isTooltipShown = false;
        tooltipUI?.Hide();
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        Gizmos.color = Color.cyan;
        if (!useCone)
        {
            Gizmos.DrawWireSphere(playerTransform.position, pickupDistance);
        }
        else
        {
            Vector3 forward = playerTransform.forward;
            Quaternion left = Quaternion.AngleAxis(-coneHalfAngle, Vector3.up);
            Quaternion right = Quaternion.AngleAxis(coneHalfAngle, Vector3.up);
            Vector3 a = (left * forward).normalized * pickupDistance;
            Vector3 b = (right * forward).normalized * pickupDistance;
            Gizmos.DrawLine(playerTransform.position, playerTransform.position + a);
            Gizmos.DrawLine(playerTransform.position, playerTransform.position + b);
        }
    }
}
