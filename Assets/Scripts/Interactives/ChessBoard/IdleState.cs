using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ChessBoard {
    public class IdleState : ChessBoardState
    {
        public override void Enter()
        {
            Debug.Log("Enter IdleState");
        }

        public override void Exit()
        {
            Debug.Log("Exit IdleState");
        }

        public override void Interact() {
            ChangeState(ActiveGameState.WaitingForPlayers);
        }
    }
}
