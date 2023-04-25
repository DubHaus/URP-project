using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ChessBoard {

    public class ChessBoardSquare : Interactive.Interactive {
        [SerializeField] public Vector2 coordinates;

        public void Highlight() {
            gameObject.layer = LayerMask.NameToLayer("Highlight");
        }
    }

}
