using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Project.Utils.Input;

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
            Debug.Log(activeInteractive);
            if (activeInteractive) {
                activeInteractive.Interact();
            }
        }

        private void DetectInteractive() {
            var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), playerInteractionDetectionRange, layermask);
            foreach (RaycastHit hit in hits) {
                if (hit.transform.gameObject.TryGetComponent(out Interactive elem)) {
                    activeInteractive = elem;
                    break;
                }
            }
        }
    }
}

