using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform idlePosition;

    public void SwitchFollowToPlayer()
    {
        vcam.Follow = playerTransform;
    }
    
    public void SwitchFollowToIdlePos()
    {
        vcam.Follow = idlePosition;
    }
}
