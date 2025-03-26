using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    private Controls inputAction;
    [SerializeField] private ManageBirds manageBirds;

    private void OnEnable()
    {
        inputAction = new Controls();
        inputAction.Enable();
        inputAction.Actions.Jump.performed += OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (manageBirds.Index >= manageBirds.Birds.Count) return;

        manageBirds.Birds[manageBirds.Index].DrawTrajectoryRecurrence();
    }
}
