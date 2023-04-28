using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;

namespace Project.ChessBoard {
    public class WaitingForPlayersState : ChessBoardState {
        public override void Enter() {

            ChessBoardUI.Instance.ShowStartButton();
            Debug.Log("Enter WaitingForPlayersState");
        }

        public override void Exit() {
            Debug.Log("Exit WaitingForPlayersState");
        }

        public override void OnJoinGame(int playersCount) {
            Debug.Log("OnJoinGame " + playersCount);
            float minPlayersCount = 2;
            if (playersCount >= minPlayersCount) {
                ChessBoardNetworkController.LocalInstance.ChangeState(ActiveGameState.ChoosingCharacter);
            }
        }

        public override void StartGame() {
            ChessBoardUI.Instance.HideStartButton();
            ChessBoardUI.Instance.ShowText("Waiting for other players");
            JoinGame();
        }
    }
}
