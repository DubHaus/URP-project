using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    public void Focus(Transform target)
    {
        cinemachineVirtualCamera.Follow = target;
        cinemachineVirtualCamera.LookAt = target;
    }
}


