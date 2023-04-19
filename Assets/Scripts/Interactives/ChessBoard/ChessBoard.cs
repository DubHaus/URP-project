using System.Collections;
using System.Collections.Generic;
using Project.CameraUtils;
using Project.Gameplay.GameplayObjects.Character;
using Project.Gameplay.UI;
using UnityEngine;
using Project.Utils.Input;


namespace Project.Interactive {

    public class GameState {
        ChessPiece activePiece;
    }

    public class ChessBoard : Interactive {

        [SerializeField] Player player;
        [SerializeField] FreeCameraSystem cameraSystem;
        [SerializeField] TipsUI tipsUI;
        [SerializeField] private LayerMask layermask;

        private ChessPiece activePiece;
        private ChessBoardState gameStateActive;
        private GameState gameState;

        private void Awake() {
            gameStateActive.UpdateState = (GameState state) => {
                gameState = state;
            };
        }


        void Start() {
            ChessPiece[] children = gameObject.GetComponentsInChildren<ChessPiece>();

            foreach (ChessPiece child in children) {
                child.OnClick += (_) => {
                    gameStateActive.ClickOnPiece(child.piece);
                };
            }
            this.OnClick += (hit) => {
                gameStateActive.ClickOnBoard("B1");
            };
        }

        void Update() {

        }

        //private void OnClick(Vector2 clickPosition) {
        //    float maxDistance = 100;
        //    if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPosition), out RaycastHit hit, maxDistance)) {
        //        Debug.Log(hit.transform);
        //        if (hit.transform.gameObject.TryGetComponent(out ChessPiece chessPiece)) {
        //            player.Move(chessPiece.transform.position);
        //            Destroy(hit.transform.gameObject);
        //        }
        //    }
        //}

        override public void Interact() {
            StartGame();
        }

        private void StartGame() {
            cameraSystem.InFocus(transform, 45, 5);
            tipsUI.ShowTip("Select character");
            player.SetMovementLocked(true);



        }
    }
}




