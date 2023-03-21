using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Project.Utils.Input;

public class FreeCameraSystem : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float minFollowOffset = 10f;
    [SerializeField] private float maxFollowOffset = 50f;
    //[SerializeField] private PlayerInputController playerInputController;

    private Vector3 followOffset;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    private void Start()
    {
        PlayerInputController.Instance.OnZoomIn += ZoomIn;
        PlayerInputController.Instance.OnZoomOut += ZoomOut;
        PlayerInputController.Instance.OnSwipe += Move;
    }



    public void Move(Vector3 direction)
    {
        // Debug.Log("MOVE CAMERA: " + direction);
        transform.position += direction;
    }

    public void ZoomIn(float zoomAmount)
    {
        // Debug.Log("ZOOM CAMERA IN:" + zoomAmount);
        followOffset.y -= zoomAmount;
        followOffset.z -= zoomAmount / 2;
        followOffset.y = Mathf.Clamp(followOffset.y, minFollowOffset, maxFollowOffset);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                followOffset,
                Time.deltaTime * zoomSpeed
            );
    }

    public void ZoomOut(float zoomAmount)
    {
        // Debug.Log("ZOOM CAMERA OUT:" + zoomAmount);
        followOffset.y += zoomAmount;
        followOffset.z += zoomAmount / 2;
        followOffset.y = Mathf.Clamp(followOffset.y, minFollowOffset, maxFollowOffset);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                followOffset,
                Time.deltaTime * zoomSpeed
            );
    }
}
