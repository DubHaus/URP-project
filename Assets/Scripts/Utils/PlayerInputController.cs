using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Project.Utils.Input {

    public enum Action {
        touch,
        swipe,
        zoom,
    }

    public class PlayerInputController : MonoBehaviour {
        public static PlayerInputController Instance { get; private set; }
        public event Action<Vector3> OnTouch;
        public event Action<Vector2> OnTouch2D;
        public event Action<Vector3> OnSwipe;
        public event Action<float> OnZoomIn;
        public event Action<float> OnZoomOut;

        [SerializeField] private float zoomSensetifity;
        [SerializeField] private float zoomAmount = 3f;

        private PlayerInputControls playerInputControls;
        private Action? currentAction = null;
        private Vector2? lastPrimaryTouchPosition;
        private float fingersLastDistance;

        private void Awake() {

            DontDestroyOnLoad(gameObject);

            if (Instance != null && Instance != this) {
                Debug.LogError("More than one PlayerInputController instance");
            }
            else {
                Instance = this;
            }

            playerInputControls = new PlayerInputControls();

            playerInputControls.Touch.PrimaryTouchContact.started += _ => {
                currentAction = Action.touch;
                lastPrimaryTouchPosition = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
            };

            playerInputControls.Touch.PrimarySwipeDelta.started += _ => {
                if (currentAction != Action.zoom) {
                    currentAction = Action.swipe;
                }
            };
            playerInputControls.Touch.SecondaryTouchContact.started += _ => {
                currentAction = Action.zoom;
                fingersLastDistance = Vector2.Distance(
                    playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>(),
                    playerInputControls.Touch.SecondaryTouchPosition.ReadValue<Vector2>()
                );
            };

            playerInputControls.Touch.SecondaryTouchContact.canceled += _ => {
                currentAction = null;
            };

            playerInputControls.Touch.PrimaryTouchContact.canceled += PrimaryTouchHandler;
            playerInputControls.Touch.PrimarySwipeDelta.performed += PrimarySwipeHandler;
            playerInputControls.Touch.SecondaryTouchPosition.performed += ZoomHandler;
        }

        private void OnEnable() {
            playerInputControls.Touch.Enable();
        }

        private void OnDisable() {
            playerInputControls.Touch.Disable();

        }

        private void PrimarySwipeHandler(InputAction.CallbackContext context) {
            if (currentAction == Action.swipe) {
                Vector2 currentTouchPosition = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();

                if (lastPrimaryTouchPosition is Vector2 valueOfLastTouchPosition) {
                    Vector3 targetPosition = GetWorldPosition(currentTouchPosition);
                    Vector3 direction = GetWorldPosition(valueOfLastTouchPosition) - targetPosition;
                    direction.y = 0;
                    OnSwipe?.Invoke(direction);
                }
                lastPrimaryTouchPosition = currentTouchPosition;
            }

        }

        private void PrimaryTouchHandler(InputAction.CallbackContext context) {
            if (currentAction == Action.touch) {
                Vector2 currentMovementInput = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
                OnTouch2D?.Invoke(currentMovementInput);
                OnTouch?.Invoke(GetWorldPosition(currentMovementInput));
                currentAction = null;
            }
        }

        private void ZoomHandler(InputAction.CallbackContext context) {
            if (currentAction == Action.zoom) {
                float fingersDistance = Vector2.Distance(
                    playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>(),
                    playerInputControls.Touch.SecondaryTouchPosition.ReadValue<Vector2>()
                );

                if ((fingersLastDistance - fingersDistance) > zoomSensetifity) {
                    OnZoomOut?.Invoke(zoomAmount);
                }
                else if ((fingersDistance - fingersLastDistance) > zoomSensetifity) {
                    OnZoomIn?.Invoke(zoomAmount);

                }
                fingersLastDistance = fingersDistance;
            }
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition) {
            RaycastHit hit;

            float maxDistance = 100;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out hit, maxDistance)) {
                return hit.point;
            }
            return Vector3.zero;

        }
    }
}
