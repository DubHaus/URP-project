using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


namespace Project.Utils.Input {

    public enum CurrentAction {
        touch,
        swipe,
        zoom,
    }

    public class PlayerInputController : MonoBehaviour {
        public static PlayerInputController Instance { get; private set; }
        public event Action<Vector3> OnClick;
        public event Action<Vector2> OnClick2D;
        public event Action<Vector3> OnSwipe;
        public event Action OnZoomIn;
        public event Action OnZoomOut;
        public event Action<Vector2> OnMove;
        public event Action<float> OnRotate;
        public event Action OnInteract;
        public event Action<Vector2> OnPoint;

        [SerializeField] private float zoomSensetifity;

        private PlayerInputControls playerInputControls;
        private CurrentAction? currentAction = null;
        private Vector2? lastPrimaryTouchPosition;
        private float fingersLastDistance;
        private float rotation = 0;
        private bool isMoving = false;
        private Vector2 moveVector = Vector2.zero;

        private void Awake() {

            DontDestroyOnLoad(gameObject);

            if (Instance != null && Instance != this) {
                Debug.LogError("More than one PlayerInputController instance");
            }
            else {
                Instance = this;
            }

            playerInputControls = new PlayerInputControls();

            // Touch
            playerInputControls.Touch.PrimaryTouchContact.started += _ => {
                currentAction = CurrentAction.touch;
                lastPrimaryTouchPosition = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
            };

            playerInputControls.Touch.PrimarySwipeDeltaPointer.started += _ => {
                if (currentAction != CurrentAction.zoom) {
                    if (playerInputControls.Touch.PrimaryTouchContact.ReadValue<float>() == 1) {
                        currentAction = CurrentAction.swipe;
                    }
                }
            };
            playerInputControls.Touch.SecondaryTouchContact.started += _ => {
                currentAction = CurrentAction.zoom;
                fingersLastDistance = Vector2.Distance(
                    playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>(),
                    playerInputControls.Touch.SecondaryTouchPosition.ReadValue<Vector2>()
                );
            };

            playerInputControls.Touch.SecondaryTouchContact.canceled += _ => {
                currentAction = null;
            };

            playerInputControls.Touch.PrimaryTouchContact.canceled += PrimaryTouchHandler;
            playerInputControls.Touch.PrimarySwipeDeltaPointer.performed += PrimarySwipeHandler;
            playerInputControls.Touch.SecondaryTouchPosition.performed += PinchHandler;

            // Mouse
            playerInputControls.MouseKeyboard.Scroll.performed += ScrollHandler;
            playerInputControls.MouseKeyboard.Move.performed += MoveHandlerStarts;
            playerInputControls.MouseKeyboard.Move.canceled+= MoveHandlerEnds;
            playerInputControls.MouseKeyboard.Rotate.performed += RotateHandlerStarted;
            playerInputControls.MouseKeyboard.Rotate.canceled += RotateHandlerEnds;
            playerInputControls.MouseKeyboard.Interact.performed += InteractHandler;
            playerInputControls.MouseKeyboard.PointerPosition.performed += PointerHandler;
        }

        private void OnEnable() {
            playerInputControls.Touch.Enable();
            playerInputControls.MouseKeyboard.Enable();
        }

        private void OnDisable() {
            playerInputControls.Touch.Disable();
            playerInputControls.MouseKeyboard.Disable();

        }

        private void Update() {
            if (rotation != 0) {
                OnRotate?.Invoke(rotation);
            }

            if(isMoving) {
                OnMove?.Invoke(moveVector);

                if(moveVector == Vector2.zero) {
                    isMoving = false;
                }
            }
        }

        private void PrimarySwipeHandler(InputAction.CallbackContext context) {
            if (playerInputControls.Touch.PrimaryTouchContact.ReadValue<float>() == 1 &&
                !EventSystem.current.IsPointerOverGameObject()) {
                Vector2 currentTouchPosition = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();

                if (lastPrimaryTouchPosition is Vector2 valueOfLastTouchPosition) {
                    Vector3 targetPosition = GetWorldPosition(currentTouchPosition);
                    Vector3 direction = GetWorldPosition(valueOfLastTouchPosition) - targetPosition;
                    if (direction != Vector3.zero) {
                        direction.y = 0;
                        OnSwipe?.Invoke(direction);
                    }
                    else {
                        currentAction = CurrentAction.touch;
                    }
                }
                lastPrimaryTouchPosition = currentTouchPosition;
            }

        }

        private void PrimaryTouchHandler(InputAction.CallbackContext context) {
            if (!EventSystem.current.IsPointerOverGameObject() & currentAction == CurrentAction.touch) {
                Vector2 currentMovementInput = playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
                OnClick2D?.Invoke(currentMovementInput);
                OnClick?.Invoke(GetWorldPosition(currentMovementInput));
                currentAction = null;
            }
        }

        private void PinchHandler(InputAction.CallbackContext context) {
            if (!EventSystem.current.IsPointerOverGameObject() & currentAction == CurrentAction.zoom) {
                float fingersDistance = Vector2.Distance(
                    playerInputControls.Touch.PrimaryTouchPosition.ReadValue<Vector2>(),
                    playerInputControls.Touch.SecondaryTouchPosition.ReadValue<Vector2>()
                );

                if ((fingersLastDistance - fingersDistance) > zoomSensetifity) {
                    OnZoomOut?.Invoke();
                }
                else if ((fingersDistance - fingersLastDistance) > zoomSensetifity) {
                    OnZoomIn?.Invoke();

                }
                fingersLastDistance = fingersDistance;
            }
        }

        private void ScrollHandler(InputAction.CallbackContext context) {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                float value = context.ReadValue<Vector2>().y;
                if (value > 0) {
                    OnZoomIn?.Invoke();
                }
                else {
                    OnZoomOut?.Invoke();
                }
            }
        }

        private void MoveHandlerStarts(InputAction.CallbackContext context) {
            Vector2 currentMovementInput = context.ReadValue<Vector2>();
            moveVector = currentMovementInput;
            isMoving = true;
        }
        private void MoveHandlerEnds(InputAction.CallbackContext context) {
            moveVector = Vector2.zero;
        }

        private void RotateHandlerStarted(InputAction.CallbackContext context) {
            float currentRotateInput = context.ReadValue<float>();
            rotation = currentRotateInput;
        }

        private void RotateHandlerEnds(InputAction.CallbackContext context) {
            rotation = 0;
        }

        private void InteractHandler(InputAction.CallbackContext context) {
            OnInteract?.Invoke();
        }

         private void PointerHandler(InputAction.CallbackContext context) {
            Vector2 pointerInput = context.ReadValue<Vector2>();
            OnPoint?.Invoke(pointerInput);
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition) {

            float maxDistance = 100;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out RaycastHit hit, maxDistance)) {
                return hit.point;
            }
            return Vector3.zero;

        }

    }
}
