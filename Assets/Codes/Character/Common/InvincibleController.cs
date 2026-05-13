using UnityEngine;
using System.Collections;

public class InvincibleController : MonoBehaviour
{
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private float flashInterval = 0.1f;

    [SerializeField] private GameObject modelRoot;

    private bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    private Renderer[] renderers;

    private Coroutine invincibleRoutine;
    private Coroutine flashRoutine;

    private void Awake()
    {
        if (modelRoot != null)
        {
            renderers = modelRoot.GetComponentsInChildren<Renderer>();
        }

    }

    public void StartInvincibility()
    {
        if (invincibleRoutine != null) StopCoroutine(invincibleRoutine);
        invincibleRoutine = StartCoroutine(Invincible());

        if (modelRoot != null)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                ToggleRenderers(true);
            }
            flashRoutine = StartCoroutine(FlashCoroutine());
        }
    }

    private IEnumerator Invincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        invincibleRoutine = null;
    }

    private IEnumerator FlashCoroutine()
    {
        float time = 0f;

        while (time < invincibleTime)
        {
            ToggleRenderers(false);
            yield return new WaitForSeconds(flashInterval);
            ToggleRenderers(true);
            yield return new WaitForSeconds(flashInterval);

            time += flashInterval * 2;
        }
        ToggleRenderers(true);
        flashRoutine = null;
    }

    private void ToggleRenderers(bool isVisible)
    {
        if (renderers == null) return;

        foreach (var r in renderers)
        {
            if (r != null)
            {
                r.enabled = isVisible;
            }
        }
    }
}