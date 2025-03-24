using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform idlePosition;

    private void Start()
    {
        SwitchFollowToIdlePos();
    }

    public void SwitchFollowBird(Transform _newFollowBird)
    {
        playerTransform = _newFollowBird;
    }
    
    public void SwitchFollowToPlayer()
    {
        vcam.Follow = playerTransform;
    }
    
    public void SwitchFollowToIdlePos()
    {
        vcam.Follow = idlePosition;
    }
}
