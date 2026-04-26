using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private CinemachineInputAxisController inputAxisController;
    private CinemachinePanTilt cinemachinePanTilt;
    [SerializeField] private float shakeOffset = 5f;
    private PlayerHealth playerHealth;

    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        inputAxisController = GetComponent<CinemachineInputAxisController>();
        cinemachinePanTilt = GetComponent<CinemachinePanTilt>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth.OnDamaged += ShakeCamera;
        playerHealth.OnDeath += StopShakeCamera;
        Instance = this;
    }

    private void Update() { }

    public void SwitchLookControll(bool state)
    {
        inputAxisController.enabled = state;
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine());
    }

    public void StopShakeCamera()
    {
        StopAllCoroutines();
        if (cinemachinePanTilt != null)
            cinemachinePanTilt.TiltAxis.Value = 0f;
    }

    private IEnumerator ShakeCameraCoroutine()
    {
        if (cinemachinePanTilt == null) yield break;
        float origin = cinemachinePanTilt.TiltAxis.Value;

        for (int i = 0; i < 3; i++)
        {
            cinemachinePanTilt.TiltAxis.Value = origin + shakeOffset;
            yield return new WaitForSeconds(0.05f);
            cinemachinePanTilt.TiltAxis.Value = origin - shakeOffset;
            yield return new WaitForSeconds(0.05f);
        }
        cinemachinePanTilt.TiltAxis.Value = origin;
    }
}

