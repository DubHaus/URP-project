using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;

namespace Project.ChessBoard {
    public class PlayingGameState : ChessBoardState {

        private ChessPiece? selectedPiece;
        private List<PieceMove> moves = new List<PieceMove>();
        public override void Enter() {
            Debug.Log("Enter PlayingGameState");
            ChessBoardUI.Instance.ShowText("Game started");
            ChessBoardNetworkController.LocalInstance.ChangeActivePlayer(); // set the first one
        }

        public override void Exit() {
            Debug.Log("Exit PlayingGameState");
        }

        public override void ClickOnPiece(ChessPiece piece) {
            if (
                ChessBoardNetworkController.LocalInstance.isLocalPlayerActive && selectedPiece != piece) {
                if (ChessBoardNetworkController.LocalInstance.localPlayer?.side == piece.pieceColor) {
                    foreach (var oldMove in moves) {
                        ChessBoardNetworkController.LocalInstance.ChessBoard.RemoveHighlight(oldMove.coordinates);
                    }
                    if (selectedPiece != null) {
                        selectedPiece.PutDown();
                    }
                    selectedPiece = piece;
                    selectedPiece.LiftUp();

                    moves = piece.GetPosibleMoves();
                    foreach (var move in moves) {
                        ChessBoardNetworkController.LocalInstance.ChessBoard.HighlightSquare(move);
                    }

                } else if (selectedPiece != null) {
                    foreach (var move in moves) {
                        if (move.coordinates == piece.currentPosition && move.type == PieceMoveType.Fight) {
                            MakeMove(new PieceMove(PieceMoveType.Fight, piece.currentPosition, piece.id));
                        }
                    }
                }
            }
        }

        public override void ClickOnBoard(ChessBoardSquare square) {
            if (
                ChessBoardNetworkController.LocalInstance.isLocalPlayerActive
                && selectedPiece
                ) {
                foreach (var move in moves) {
                    if (move.coordinates == square.coordinates) {
                        MakeMove(move);
                    }
                }
            }

        }

        private void MakeMove(PieceMove move) {
            switch (move.type) {
                case PieceMoveType.Fight:
                    if (move.pieceId != null) {
                        ChessBoardNetworkController.LocalInstance.RemovePiece((int)move.pieceId);
                        ChessBoardNetworkController.LocalInstance.UpdatePiecePosition(selectedPiece.id, move.coordinates);
                    }
                    break;
                case PieceMoveType.Move:
                    ChessBoardNetworkController.LocalInstance.UpdatePiecePosition(selectedPiece.id, move.coordinates);
                    break;

            }
            ChessBoardNetworkController.LocalInstance.ChangeActivePlayer();

            foreach (var oldMove in moves) {
                ChessBoardNetworkController.LocalInstance.ChessBoard.RemoveHighlight(oldMove.coordinates);
            }
            selectedPiece = null;
        }

        public override void OnPlayersTurn() {
            if (ChessBoardNetworkController.LocalInstance.isLocalPlayerActive) {
                ChessBoardUI.Instance.ShowText("Your turn. Make a move");
            } else {
                ChessBoardUI.Instance.ShowText("Your opponent's turn");
            }
        }

        public override void OnPlayersKilled(ulong playerId) {
            ChessBoardNetworkController.LocalInstance.SetLoserPlayerId(playerId);
            ChessBoardNetworkController.LocalInstance.ChangeState(ActiveGameState.EndGame);
        }

    }
}
