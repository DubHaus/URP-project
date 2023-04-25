using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Project.ChessBoard {
    abstract public class ChessBoardState {
        public abstract void Enter();
        public abstract void Exit();


        public Action JoinGame { get; set; }
        public Action LeaveGame { get; set; }
        public Action<ActiveGameState> ChangeState { get; set; }

        public virtual void Interact() { }
        public virtual void StartGame() { }
        public virtual void ClickOnPiece(ChessPiece piece) { }
        public virtual void ClickOnBoard(ChessBoardSquare square) { }

        public virtual void OnJoinGame(int playerCount) { }
        public virtual void OnPlayerReady(bool allPlayersReady) { }
    }

}

