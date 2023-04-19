using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Interactive {
    public class ChoosingCharacterState : ChessBoardState
    {
        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void ClickOnPiece(ChessPiceType piece)
        {
            this.SaveState(new GameState());
        }

    }
}
