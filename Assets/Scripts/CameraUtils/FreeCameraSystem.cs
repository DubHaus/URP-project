using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Project.Utils.Input;
using VContainer;
using UnityEngine.UIElements;
using static UnityEngine.GridBrushBase;

namespace Project.CameraUtils {

    public class FreeCameraSystem : MonoBehaviour {
        static public FreeCameraSystem Instance { get; private set; }

        [SerializeField] private float zoomAmount = 1;
        [SerializeField] private float rotateAmount = 5;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private Vector3 minFollowOffset = new Vector3(1, 5, 0.5f);
        [SerializeField] private Vector3 maxFollowOffset = new Vector3(3, 10, 2);


        private Vector3 followOffset;
        private bool flippedCameraAngle = false;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Debug.LogError("More than one FreeCameraSystem instance");
            } else {
                Instance = this;
            }

            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Start() {
            PlayerInputController.Instance.OnZoomIn += OnZoomIn;
            PlayerInputController.Instance.OnZoomOut += OnZoomOut;
            PlayerInputController.Instance.OnSwipe += Move;
            PlayerInputController.Instance.OnRotate += OnRotate;
        }

        private void OnDestroy() {
            PlayerInputController.Instance.OnZoomIn -= OnZoomIn;
            PlayerInputController.Instance.OnZoomOut -= OnZoomOut;
            PlayerInputController.Instance.OnSwipe -= Move;
            PlayerInputController.Instance.OnRotate -= OnRotate;
            Instance = null;
        }

        private void OnZoomIn() {
            ZoomIn(zoomAmount);
        }

        private void OnZoomOut() {
            ZoomOut(zoomAmount);
        }
        private void OnRotate(float direction) {
            Rotate(direction, rotateAmount);
        }



        public void Move(Vector3 direction) {
            transform.position += direction;
        }

        public void UpdatePosition(Vector3 position) {
            float updatePositionSpeed = 3f;
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(position.x, transform.position.y, position.z),
                Time.deltaTime * updatePositionSpeed
                );
        }

        public void ZoomIn(float zoomAmount) {
            float followOffsetY =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>()
                .m_FollowOffset.y;
            SetZoom(followOffsetY - zoomAmount);
        }

        public void ZoomOut(float zoomAmount) {

            float followOffsetY =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>()
                .m_FollowOffset.y;
            SetZoom(followOffsetY + zoomAmount);
        }

        private void SetZoom(float zoom) {
            Vector3 followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
            followOffset.y = Mathf.Clamp(zoom, 3, 12);
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
            //Vector3.Lerp(
            //    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
            //    followOffset,
            //    Time.deltaTime * zoomSpeed
            //);
        }

        public void Rotate(float rotationDirection, float amount) {
            Vector3 newRotation = transform.eulerAngles;
            newRotation.y += amount * rotationDirection;

            transform.eulerAngles = newRotation;
        }

        public void InFocus(UnityEngine.Transform target, float rotation = 0, float zoom = 0) {

            transform.position = new Vector3(target.position.x, 0, target.position.z);

            if (zoom != 0) {
                SetZoom(zoom);
            }
            if (rotation != 0) {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y = rotation;

                transform.eulerAngles = newRotation;
            }
        }

        public void Focus(Transform target) {
            cinemachineVirtualCamera.Follow = target;
            cinemachineVirtualCamera.LookAt = target;
        }
    }
}

