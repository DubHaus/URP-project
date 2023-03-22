using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Project.Gameplay.GameplayObjects.Character {

    public class PlayerAnimatior : NetworkBehaviour {

        private const string IS_WALKING = "IsWalking";

        [SerializeField] private Player player;
        private Animator playerAnimator;

        private void Awake() {
            playerAnimator = GetComponent<Animator>();
        }

        private void Update() {
            if (!IsOwner) {
                return;
            }
            playerAnimator.SetBool(IS_WALKING, player.IsWalking());
        }
    }
}
