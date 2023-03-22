using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Project.Utils.Input;
using VContainer;
using Project.VoiceChatUtils;

namespace Project.Gameplay.GameplayObjects.Character {

    public class Player : NetworkBehaviour {
        private NavMeshAgent agent;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if(!IsOwner) {
                AgoraVoiceController.Instance.UpdateSpatialAudioPosition(transform.position);
            }
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (IsOwner) {
                PlayerInputController.Instance.OnTouch += Move;
            }
        }

        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            if (IsOwner) {
                PlayerInputController.Instance.OnTouch -= Move;
            }
        }

        public void Move(Vector3 targetPosition) {
            if (IsOwner) {
                agent.SetDestination(targetPosition);
            }
        }

        public bool IsWalking() {
            return agent.hasPath;
        }
    }
}

