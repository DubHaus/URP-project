using System.Collections;
using System.Collections.Generic;
using Project.CameraUtils;
using Project.Gameplay.GameplayObjects.Character;
using Project.Gameplay.UI;
using Unity.Netcode;
using UnityEngine;


namespace Project.ChessBoard {

    public class ChessBoard : MonoBehaviour {

        [SerializeField] GameObject ChessPiecePrefab;

        [SerializeField] ChessBoardNetworkController ChessBoardNetworkController;
        [SerializeField] FreeCameraSystem cameraSystem;

        private ChessBoardSquare[,] squares = new ChessBoardSquare[8, 8];
        private List<ChessPiece> piecesGO = new List<ChessPiece>();

        void Start() {
            ChessBoardSquare[] squaresGo = gameObject.GetComponentsInChildren<ChessBoardSquare>();

            foreach (var square in squaresGo) {
                squares[(int)square.coordinates.x, (int)square.coordinates.y] = square;
                square.clickable.OnClick += (_) => {
                    ChessBoardNetworkController.ActiveState.ClickOnBoard(square);
                };
            }
        }

        public void InstantiatePieces(NetworkList<Piece> pieces) {
            foreach (var piece in pieces) {
                GeneratePieceGO(piece);
            }

        }

        public void GeneratePieceGO(Piece piece) {
            var prefab = Instantiate(ChessPiecePrefab, transform);
            if (prefab.TryGetComponent(out ChessPiece chessPiece)) {
                chessPiece.id = piece.id;
                chessPiece.pieceColor = piece.color;
                chessPiece.piece = piece.type;
                chessPiece.currentPosition = piece.position;
                chessPiece.InitPiece();
                chessPiece.clickable.OnClick += (_) => {
                    ChessBoardNetworkController.ActiveState.ClickOnPiece(chessPiece);
                };

                piecesGO.Add(chessPiece);
            } else {
                Debug.LogError("ChessPiecePrefab do not have ChessPiece component");
            }
        }

        public void UpdatePieces(NetworkList<Piece> pieces) {
            Debug.Log("UpdatePieces pieces " + pieces.Count + "; piecesGO " + piecesGO.Count);
            if (pieces.Count < piecesGO.Count) {
                List<int> pieceIds = new List<int>();
                foreach (var piece in pieces) {
                    pieceIds.Add(piece.id);
                }

                piecesGO.RemoveAll(pieceGO => {
                    if (!pieceIds.Contains(pieceGO.id)) {
                        if (pieceGO.controledByPlayer) {
                            ChessBoardNetworkController.LocalInstance.ActiveState.OnPlayersKilled(
                                ChessBoardNetworkController.LocalInstance.NetworkManager.LocalClientId
                            );
                        }
                        Destroy(pieceGO.transform.gameObject);
                        return true;
                    }
                    return false;
                });
                Debug.Log("Remove pieces " + pieces.Count + "; piecesGO " + piecesGO.Count);
            }

            foreach (var piece in pieces) {
                var matchedPiece = piecesGO.Find((pieceGO) => pieceGO.id == piece.id);
                if (matchedPiece) {
                    matchedPiece.pieceColor = piece.color;
                    matchedPiece.piece = piece.type;
                    matchedPiece.Move(piece.position);
                } else {
                    GeneratePieceGO(piece);
                }

            }
        }


        public void HighlightSquare(PieceMove pieceMove) {
            switch (pieceMove.type) {
                case PieceMoveType.Fight:
                    squares[(int)pieceMove.coordinates.x, (int)pieceMove.coordinates.y]?.HighlightRed();
                    break;
                case PieceMoveType.Move:
                    squares[(int)pieceMove.coordinates.x, (int)pieceMove.coordinates.y]?.Highlight();
                    break;
            }

        }

        public void RemoveHighlight(Vector2 squareCoords) {
            squares[(int)squareCoords.x, (int)squareCoords.y]?.RemoveHighlight();
        }

        public void Interact() {
            cameraSystem.InFocus(transform, 45, 5);
            Gameplay.GameplayObjects.Character.Player.LocalInstance.SetMovementLocked(true);
        }

        public void ChooseCharacter(ChessPiceType character, ChessPicesColor side) {
            var matchedPiece = piecesGO.Find((pieceGO) => pieceGO.pieceColor == side && pieceGO.piece == character);
            matchedPiece.SetAsPlayerCharacter();
        }

    }
}




