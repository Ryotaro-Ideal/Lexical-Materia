using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineInputAxisController inputAxisController;

    public static CameraController Instance { get; private set; }
    private void Awake()
    {
        inputAxisController = GetComponent<CinemachineInputAxisController>();
        Instance = this;
    }

    public void OnMove(InputAction.CallbackContext context)
    {

    }

    private void Update()
    {


    }
    public void SwitchLookControll(bool state)
    {
        inputAxisController.enabled = state;

    }

}
