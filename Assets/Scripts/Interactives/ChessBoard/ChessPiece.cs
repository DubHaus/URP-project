using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interactive;

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

    public class ChessPiece : Interactive.Interactive {

        [SerializeField] public ChessPiceType piece;
        [SerializeField] public ChessPicesColor pieceColor;

        public Vector2 squareMatrixPosition { get; private set; }

        public int id;
        private Vector3 originalPosition;
        private Vector3 newPosition;
        public bool lifted = false;

        private void Awake() {
            int firstRowIndex = pieceColor == ChessPicesColor.Black ? 0 : 8;

            switch (piece) {
                case ChessPiceType.Bishop:
                    squareMatrixPosition = new Vector2(firstRowIndex, 0);
                    break;
                case ChessPiceType.Knight:
                    squareMatrixPosition = new Vector2(firstRowIndex, 1);
                    break;
                case ChessPiceType.Rook:
                    squareMatrixPosition = new Vector2(firstRowIndex, 2);
                    break;
                case ChessPiceType.King:
                    squareMatrixPosition = new Vector2(firstRowIndex, 3);
                    break;
                case ChessPiceType.Queen:
                    squareMatrixPosition = new Vector2(firstRowIndex, 4);
                    break;
                case ChessPiceType.Rook2:
                    squareMatrixPosition = new Vector2(firstRowIndex, 5);
                    break;
                case ChessPiceType.Knight2:
                    squareMatrixPosition = new Vector2(firstRowIndex, 6);
                    break;
                case ChessPiceType.Bishop2:
                    squareMatrixPosition = new Vector2(firstRowIndex, 7);
                    break;
            }

        }

        public void InitPosition(Vector2 position) {
            squareMatrixPosition = position;
            transform.localPosition = CalculateLocalPosition(position);
        }

        // Update is called once per frame
        void Update() {
        }


        public void LiftUp() {
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

        public List<Vector2> GetPosibleMoves() {
            var moves = new List<Vector2>();
            moves.Add(squareMatrixPosition + new Vector2(0, 1));

            switch (piece) {
                default:
                    return moves;
            };

        }

        public void Move(Vector2 coordinates) {
            squareMatrixPosition = coordinates;
            originalPosition = transform.localPosition;
            newPosition = CalculateLocalPosition(coordinates);
            Debug.Log("Move Piece " + newPosition + "; squareMatrixPosition " + squareMatrixPosition[0] + "-" + squareMatrixPosition[1] + "; " + coordinates);
            StartCoroutine(LerpTransition());
            transform.localPosition = newPosition;
        }

        private Vector3 CalculateLocalPosition(Vector2 coordinates) {
            Vector3 localPosition = new Vector3();
            localPosition.x = coordinates.x * 0.125f;
            localPosition.y = 0;
            localPosition.z = coordinates.y * 0.125f;

            return localPosition;
        }

        IEnumerator LerpTransition() {
            float timeElapsed = 0;
            float lerpDuration = 0.2f;
            while (timeElapsed < lerpDuration) {
                transform.localPosition = Vector3.Lerp(originalPosition, newPosition, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = newPosition;
        }

    }
}
