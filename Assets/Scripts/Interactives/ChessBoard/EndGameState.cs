using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;

namespace Project.ChessBoard {
    public class EndGameState : ChessBoardState {

        public override void Enter() {
            Debug.Log("Enter EndGameState");
            bool isLocalPlayerLoser = ChessBoardNetworkController.LocalInstance.gameState.Value.loserPlayerId == ChessBoardNetworkController.LocalInstance.NetworkManager.LocalClientId;
            ChessBoardUI.Instance.ShowText("Game ends! " + (isLocalPlayerLoser ? "You lose :( " : "You're win!!! Congrats!"));

            Gameplay.GameplayObjects.Character.Player.LocalInstance.SetMovementLocked(false);
            Gameplay.GameplayObjects.Character.Player.LocalInstance.MoveWASD(new Vector2(4, 4));
        }

        public override void Exit() {
            Debug.Log("Exit EndGameState");
        }

    }
}
