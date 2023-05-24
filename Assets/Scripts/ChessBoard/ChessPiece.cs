using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Project.Gameplay;

namespace Project.ChessBoard {

    public enum ChessPiceType {
        Bishop,
        Knight,
        Rook,
        King,
        Queen,
        Rook2,
        Knight2,
        Bishop2,

    };

    public enum PieceMoveType {
        Fight,
        Move
    }

    public struct PieceMove {
        public PieceMoveType type;
        public Vector2 coordinates;
        public int? pieceId;

        public PieceMove(PieceMoveType type, Vector2 coordinates, int? pieceId) {
            this.type = type;
            this.coordinates = coordinates;
            this.pieceId = pieceId;
        }
    }

    public class ChessPiece : Interactive.Interactive {

        [SerializeField] Material whiteMaterial;
        [SerializeField] Material blackMaterial;

        public ChessPiceType piece;
        public ChessPicesColor pieceColor;

        public Vector2 currentPosition;

        public int id;
        public bool lifted = false;
        public bool controledByPlayer = false;

        private Vector3 originalPosition;
        private Vector3 newPosition;

        private void Start() {
        }

        public void InitPiece() {
            transform.localPosition = CalculateLocalPosition(currentPosition);
            GetComponent<Renderer>().material = pieceColor == ChessPicesColor.White ? whiteMaterial : blackMaterial;

        }

        // Update is called once per frame
        void Update() {
        }


        public void LiftUp() {
            if (lifted) return;
            float liftAmount = 2f;
            originalPosition = transform.localPosition;
            newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + liftAmount, transform.localPosition.z);
            StartCoroutine(LerpTransition());
            lifted = true;
        }

        public void PutDown() {
            newPosition = originalPosition;
            originalPosition = transform.localPosition;
            StartCoroutine(LerpTransition());
            lifted = false;
        }

        public List<PieceMove> GetPosibleMoves() {
            var moves = new List<Vector2>();


            switch (piece) {
                case ChessPiceType.Bishop:
                case ChessPiceType.Bishop2:
                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x + 2, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x + 3, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x + 4, currentPosition.y));

                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x - 2, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x - 3, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x - 4, currentPosition.y));

                    moves.Add(new Vector2(currentPosition.x , currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y + 2));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y + 3));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y + 4));

                    moves.Add(new Vector2(currentPosition.x, currentPosition.y - 1));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y - 2));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y - 3));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y - 4));
                    break;

                case ChessPiceType.Knight:
                case ChessPiceType.Knight2:
                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y + 2));
                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y + 2));

                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y - 2));
                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y - 2));

                    moves.Add(new Vector2(currentPosition.x + 2, currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x + 2, currentPosition.y - 1));

                    moves.Add(new Vector2(currentPosition.x - 2, currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x - 2, currentPosition.y - 1));
                    break;

                case ChessPiceType.Rook:
                case ChessPiceType.Rook2:
                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y - 1));

                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y - 1));
                    break;

                case ChessPiceType.King:
                case ChessPiceType.Queen:
                    moves.Add(new Vector2(currentPosition.x + 1, currentPosition.y));
                    moves.Add(new Vector2(currentPosition.x - 1, currentPosition.y));

                    moves.Add(new Vector2(currentPosition.x, currentPosition.y + 1));
                    moves.Add(new Vector2(currentPosition.x, currentPosition.y - 1));
                    break;
            };


            List<PieceMove> validMoves = new List<PieceMove>();

            foreach (var move in moves) {
                if (move.x > 7 || move.y > 7 || move.x < 0 || move.y < 0) {
                    continue;
                } else {
                    (PieceOverlap overlapType, Piece? piece) = ChessBoardNetworkController.LocalInstance.CheckPieceOverlap(pieceColor, move);
                    switch (overlapType) {
                        case PieceOverlap.Friendly:
                            break;
                        case PieceOverlap.Enemy:
                            validMoves.Add(new PieceMove(PieceMoveType.Fight, move, piece.Value.id));
                            break;
                        case PieceOverlap.NoOverlap:
                            validMoves.Add(new PieceMove(PieceMoveType.Move, move, null));
                            break;
                    }
                }
            }

            return validMoves;

        }

        public void Move(Vector2 coordinates) {
            currentPosition = coordinates;
            originalPosition = transform.localPosition;
            newPosition = CalculateLocalPosition(coordinates);
            Debug.Log("Move Piece " + newPosition + "; position " + currentPosition[0] + "-" + currentPosition[1] + "; " + coordinates);
            StartCoroutine(LerpTransition());
            transform.localPosition = newPosition;
        }

        public void SetAsPlayerCharacter() {
            controledByPlayer = true;

            GetComponent<Renderer>().enabled = false;
            if (NetworkManager.LocalClient.PlayerObject.TryGetComponent<Gameplay.GameplayObjects.Character.Player>(out var localPlayer)) {
                localPlayer.SyncPosition(transform);
            }
        }

        private Vector3 CalculateLocalPosition(Vector2 coordinates) {
            Vector3 localPosition = new Vector3();
            localPosition.x = coordinates.x * 0.125f + 0.03125f;
            localPosition.y = 0;
            localPosition.z = coordinates.y * 0.125f + 0.03125f;

            return localPosition;
        }

        IEnumerator LerpTransition() {
            float timeElapsed = 0;
            float lerpDuration = 0.2f;
            while (timeElapsed < lerpDuration) {
                transform.localPosition = Vector3.Lerp(originalPosition, newPosition, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                if (controledByPlayer) {
                    if (NetworkManager.LocalClient.PlayerObject.TryGetComponent<Gameplay.GameplayObjects.Character.Player>(out var localPlayer)) {
                        localPlayer.SyncPosition(transform);
                    }
                }
                yield return null;
            }
            transform.localPosition = newPosition;
        }

    }
}
