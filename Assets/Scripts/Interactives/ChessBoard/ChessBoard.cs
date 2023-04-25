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
            Debug.Log(ChessPiecePrefab.gameObject);
            foreach (var piece in pieces) {
                var prefab = Instantiate(ChessPiecePrefab, transform);
                if (prefab.TryGetComponent(out ChessPiece chessPiece)) {
                    chessPiece.id = piece.id;
                    chessPiece.pieceColor = piece.color;
                    chessPiece.piece = piece.type;
                    chessPiece.InitPosition(piece.position);
                    chessPiece.clickable.OnClick += (_) => {
                        ChessBoardNetworkController.ActiveState.ClickOnPiece(chessPiece);
                    };

                    piecesGO.Add(chessPiece);
                } else {
                    Debug.LogError("ChessPiecePrefab do not have ChessPiece component");
                }
            }

        }

        public void UpdatePieces(NetworkList<Piece> pieces) {
            foreach (var piece in pieces) {
                var matchedPiece = piecesGO.Find((pieceGO) => pieceGO.pieceColor == piece.color && pieceGO.piece == piece.type);
                matchedPiece.pieceColor = piece.color;
                matchedPiece.piece = piece.type;
                matchedPiece.Move(piece.position);
            }

        }


        public void HighlightSquare(Vector2 squareCoords) {
            Debug.Log("HighlightSquare " + squareCoords[0] + " " + squareCoords[1]);
            squares[(int)squareCoords.x, (int)squareCoords.y]?.Highlight();
        }


        public void Interact() {
            cameraSystem.InFocus(transform, 45, 5);
            Gameplay.GameplayObjects.Character.Player.LocalInstance.SetMovementLocked(true);
        }

    }
}




