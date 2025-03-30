using UnityEngine;
using UnityEngine.InputSystem;

public class Capacities : MonoBehaviour
{
    private Controls inputAction;
    [SerializeField] private ManageBirds manageBirds;
    [SerializeField] private DuplicationBirds duplicationBirds;

    private void OnEnable()
    {
        inputAction = new Controls();
        inputAction.Enable();
        inputAction.Actions.Jump.performed += OnJumpPerformed;
        inputAction.Actions.Duplication.performed += OnDuplicationPerformed;
    }
    
    private void OnDisable()
    {
        inputAction.Actions.Jump.canceled -= OnJumpPerformed;
        inputAction.Actions.Duplication.canceled -= OnDuplicationPerformed;
        inputAction.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (manageBirds.Index >= manageBirds.Birds.Count) return;

        manageBirds.Birds[manageBirds.Index].DrawTrajectoryRecurrence();
    }
    
    private void OnDuplicationPerformed(InputAction.CallbackContext context)
    {
        if (manageBirds.Index >= manageBirds.Birds.Count) return;

        duplicationBirds.DrawMultipleTrajectories(manageBirds.Birds[manageBirds.Index]);
    }
}
