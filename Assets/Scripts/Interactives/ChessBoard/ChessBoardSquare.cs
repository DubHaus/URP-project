using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ChessBoard {

    public class ChessBoardSquare : Interactive.Interactive {
        [SerializeField] public Vector2 coordinates;
        private int originalLayer;

        private void Awake() {
            originalLayer = gameObject.layer;
        }

        public void Highlight() {
            gameObject.layer = LayerMask.NameToLayer("Highlight");
        }

        public void HighlightRed() {
            gameObject.layer = LayerMask.NameToLayer("HighlightRed");
        }

        public void RemoveHighlight() {
            gameObject.layer = originalLayer;
        }
    }

}
