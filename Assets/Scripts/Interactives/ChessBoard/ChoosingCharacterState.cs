using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Project.ChessBoard {
    public class ChoosingCharacterState : ChessBoardState {
        public override void Enter() {
            Debug.Log("Enter ChoosingCharacterState");
            ChessBoardUI.Instance.ShowText("Select character");
        }

        public override void Exit() {
            Debug.Log("Exit ChoosingCharacterState");
        }

        public override void ClickOnPiece(ChessPiece piece) {
            if (!piece.lifted) {
                piece.LiftUp();
                ChessBoardNetworkController.Instance.UpdatePlayerCharacter(piece.piece);
                ChessBoardUI.Instance.ShowStartButton();
            }
        }

        public override void OnPlayerReady(bool allPlayersReady) {
            Debug.Log("OnPlayerReady " + allPlayersReady);
            if(allPlayersReady) {
                ChessBoardUI.Instance.ShowText("Game is starting...");
                ChangeState(ActiveGameState.PlayingGame);
            } else {
                ChessBoardUI.Instance.ShowText("Ready. Waiting for others...");
            }
        }


        public override void StartGame() {
            ChessBoardUI.Instance.HideStartButton();
            ChessBoardNetworkController.Instance.UpdatePlayerState(PlayerState.Ready);
        }

    }
}
