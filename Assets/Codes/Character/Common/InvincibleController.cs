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

    private void Awake()
    {
        GameObject root = modelRoot != null ? modelRoot : gameObject;
        renderers = root.GetComponentsInChildren<Renderer>();
    }

    public void StartInvincibility()
    {
        if (!isInvincible)
        {
            StartCoroutine(Invincible());
        }
    }

    private IEnumerator Invincible()
    {
        isInvincible = true;
        float time = 0f;

        while (time < invincibleTime)
        {
            ToggleRenderers(false);
            yield return new WaitForSeconds(flashInterval);
            ToggleRenderers(true);
            yield return new WaitForSeconds(flashInterval);

            time += flashInterval * 2;
        }

        // 最後に確実に表示状態に戻す
        isInvincible = false;
        ToggleRenderers(true);
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