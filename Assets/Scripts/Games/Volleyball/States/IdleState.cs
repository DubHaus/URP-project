using Games.Volleyball;
using Project.Games.API;

namespace Project.Games.Volleyball {
    public class IdleState : GameState {
        GameController m_GameController;

        public override void Enter() {
        }
        public override bool CanPlayerJoin => true;

        public IdleState(GameController gameController) {
            m_GameController = gameController;
        }

        public override void PlayerJoined(PlayerInfo player) {
            m_GameController.SetGameState(m_GameController.m_WaitingForPlayersState);
        }

        public override void Exit() {
        }
    }
}