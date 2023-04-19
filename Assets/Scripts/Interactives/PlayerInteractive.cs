using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Project.Utils.Input;
//using static Codice.Client.Common.WebApi.WebApiEndpoints;

namespace Project.Interactive {

    public class PlayerInteractive : NetworkBehaviour {
        [SerializeField] float playerInteractionDetectionRange = 5;
        [SerializeField] private LayerMask layermask;

        private Interactive activeInteractive;

        private void Start() {
            PlayerInputController.Instance.OnInteract += Interact;
        }

        private void FixedUpdate() {
            DetectInteractive();
        }

        private void Interact() {
            if (activeInteractive) {
                activeInteractive.Interact();
            }
        }

        private void DetectInteractive() {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, playerInteractionDetectionRange, layermask)) {
                hit.transform.gameObject.TryGetComponent(out activeInteractive);
            }
        }
    }
}

