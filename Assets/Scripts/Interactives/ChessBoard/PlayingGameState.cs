using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;

namespace Project.ChessBoard {
    public class PlayingGameState : ChessBoardState {

        private ChessPiece selectedPiece;
        public override void Enter() {
            Debug.Log("Enter PlayingGameState");
            ChessBoardUI.Instance.ShowText("Game started");
        }

        public override void Exit() {
            Debug.Log("Exit PlayingGameState");
        }

        public override void ClickOnPiece(ChessPiece piece) {
            selectedPiece = piece;
            selectedPiece.LiftUp();

            var moves = piece.GetPosibleMoves();
            foreach (var move in moves) {
                ChessBoardNetworkController.Instance.ChessBoard.HighlightSquare(move);
            }
        }

        public override void ClickOnBoard(ChessBoardSquare square) {
            Debug.Log("ClickOnBoard " + square);
            if (selectedPiece && selectedPiece.GetPosibleMoves().Contains(square.coordinates)) {
                ChessBoardNetworkController.Instance.UpdatePiecePosition(selectedPiece.id, square.coordinates);
            }

        }

    }
}
