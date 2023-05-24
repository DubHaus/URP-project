using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Project.ChessBoard {
    public class ChoosingCharacterState : ChessBoardState {

        private ChessPiceType selectedCharacter;
        public override void Enter() {
            Debug.Log("Enter ChoosingCharacterState");
            ChessBoardUI.Instance.ShowText("Select character");
        }

        public override void Exit() {
            Debug.Log("Exit ChoosingCharacterState");
        }

        public override void ClickOnPiece(ChessPiece piece) {
            if (ChessBoardNetworkController.LocalInstance.localPlayer?.side == piece.pieceColor) {
                if (!piece.lifted) {
                    selectedCharacter = piece.piece;
                    piece.LiftUp();
                    ChessBoardUI.Instance.ShowStartButton();
                }
            }
        }

        public override void OnPlayerReady(bool allPlayersReady) {
            Debug.Log("OnPlayerReady " + allPlayersReady);
            if (allPlayersReady) {
                ChessBoardUI.Instance.ShowText("Game is starting...");
                ChessBoardNetworkController.LocalInstance.ChangeState(ActiveGameState.PlayingGame);
            } else {
                ChessBoardUI.Instance.ShowText("Ready. Waiting for others...");
            }
        }


        public override void StartGame() {
            ChessBoardUI.Instance.HideStartButton();
            ChessBoardNetworkController.LocalInstance.UpdatePlayerCharacter(selectedCharacter);
            ChessBoardNetworkController.LocalInstance.UpdatePlayerState(PlayerState.Ready);
        }

    }
}
