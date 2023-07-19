using Games.Volleyball;
using Project.Games.API;

namespace Project.Games.Volleyball {
    public class WaitingForPlayersState : GameState {
        GameController m_GameController;

        public override bool CanPlayerJoin => true;

        public override void Enter() {
        }
        public WaitingForPlayersState(GameController gameController) {
            m_GameController = gameController;
        }

        public override void PlayerJoined(PlayerInfo player) {
            if (m_GameController.Players.Count >= GameController.minPlayers) {
                m_GameController.SetGameState(m_GameController.m_PlayingState);
            }
        }
        public override void Exit() {
        }
    }
}