using UnityEngine;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionName = "開ける";
    [SerializeField] private Transform pivot;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.5f;

    private bool isOpen = false;
    private bool isAnimating = false;

    public virtual void Awake()
    {

    }

    public virtual void Interact()
    {
        if (isAnimating) return;
        isOpen = !isOpen;
        interactionName = isOpen ? "閉める" : "開ける";
        if (doorCollider != null)
            doorCollider.isTrigger = isOpen;
        StartCoroutine(RotateDoor(isOpen ? openAngle : 0f));
    }

    private IEnumerator RotateDoor(float targetAngle)
    {
        isAnimating = true;

        Quaternion startRot = pivot.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, targetAngle, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pivot.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            yield return null;
        }

        pivot.localRotation = endRot;
        isAnimating = false;

        if (!isOpen && doorCollider != null)
            doorCollider.isTrigger = false;
    }

    public virtual string GetName() => interactionName;

    public Vector3 GetPos() => transform.position;
}
