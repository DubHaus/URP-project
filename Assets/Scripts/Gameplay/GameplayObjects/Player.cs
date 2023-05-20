using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Project.Utils.Input;
using VContainer;
using Project.VoiceChatUtils;
using Project.CameraUtils;

namespace Project.Gameplay.GameplayObjects.Character {

    public class Player : NetworkBehaviour {

        static public Player LocalInstance { get; private set; }

        [SerializeField] float playerInteractionDetectionRange = 5;
        [SerializeField] private LayerMask layermask;

        private NavMeshAgent agent;
        private Vector3 movementVector = Vector3.zero;
        private bool movementLocked = false;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        //private void Start() {
        //    PlayerInputController.Instance.OnClick += OnMove; // TODO Delete when testing online
        //    PlayerInputController.Instance.OnMove += OnMoveWASD; // TODO Delete when testing online
        //}
        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            Debug.Log("Player IsOwner " + IsOwner);
            Debug.Log("Player IsLocalPlayer " + IsLocalPlayer);
            Debug.Log("Player IsClient " + IsClient);
            Debug.Log("Player IsHost " + IsHost);
            Debug.Log("Player IsServer " + IsServer);

            if (IsOwner) {
                if (LocalInstance != null) {
                    Debug.LogError("More than one local Player instance");
                } else {
                    LocalInstance = this;
                }
                PlayerInputController.Instance.OnClick += OnMove;
                PlayerInputController.Instance.OnMove += OnMoveWASD;
                GetComponent<AudioListener>().enabled = true;
            }
        }


        private void Update() {
            if (IsOwner && IsWalking()) {
                FreeCameraSystem.Instance.UpdatePosition(transform.position);
            }
            if (!IsOwner) {
                AgoraVoiceController.Instance.UpdateSpatialAudioPosition(transform.position);
            }
        }


        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            if (IsOwner) {
                PlayerInputController.Instance.OnClick -= Move;
            }
        }

        private void OnMove(Vector3 targetPosition) {
            if (movementLocked) return;
            Move(targetPosition);
        }

        private void OnMoveWASD(Vector2 directions) {
            if (movementLocked) return;
            MoveWASD(directions);
        }

        public void Move(Vector3 targetPosition) {
            if (IsOwner) {
                agent.SetDestination(targetPosition);
            }
        }

        public void MoveWASD(Vector2 directions) {
            if (IsOwner) {
                movementVector = new Vector3(directions.x, 0, directions.y);

                if (movementVector != Vector3.zero) {
                    agent.ResetPath();
                    Vector3 forward = Camera.main.transform.forward;
                    forward.y = 0;
                    forward = forward.normalized;
                    Vector3 right = Camera.main.transform.right;
                    right.y = 0;
                    right = right.normalized;

                    Vector3 normalizedforwardMovement = forward * movementVector.z;
                    Vector3 normalizedSideMovement = right * movementVector.x;
                    Vector3 normalizedMovement = normalizedforwardMovement + normalizedSideMovement;
                    agent.Move(normalizedMovement * agent.speed * Time.deltaTime);
                    transform.rotation = Quaternion.LookRotation(normalizedMovement);
                }

            }
        }

        public void SyncPosition(Transform parent) {
            gameObject.transform.position = parent.position;
            gameObject.transform.rotation = parent.rotation;
        }

        public void SetMovementLocked(bool value) {
            movementLocked = value;
        }


        public void Rotate(Vector3 rotation) {
            this.transform.eulerAngles = rotation;
        }

        public void LookAt(Vector3 target) {
            transform.rotation = Quaternion.LookRotation(target);
        }

        public bool IsWalking() {
            return agent.hasPath || movementVector != Vector3.zero;
        }
    }
}

