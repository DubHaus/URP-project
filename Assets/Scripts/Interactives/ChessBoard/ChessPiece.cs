using System.Collections;
using System.Collections.Generic;
using Project.Utils;
using Project.Utils.Input;
using UnityEngine;

namespace Project.Interactive {

    public enum ChessPiceType {
        King,
        Queen,
        Rook,
        Bishop,
        Knight
    };

    public class ChessPiece : Clickable {

        [SerializeField] public ChessPiceType piece;

        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
