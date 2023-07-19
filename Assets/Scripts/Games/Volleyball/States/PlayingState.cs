using Games.Volleyball;
using Project.Games.API;

namespace Project.Games.Volleyball {
    public class PlayingState : GameState {
        GameController m_GameController;
        PlayersManager m_PlayersManager;

        public override bool CanPlayerJoin => true;
        public PlayingState(GameController gameController, PlayersManager playersManager) {
            m_GameController = gameController;
            m_PlayersManager = playersManager;
        }

        public override void Enter() {
            m_PlayersManager.InitPlayers();
            m_GameController.StartGame();
        }

        public override void PlayerJoined(PlayerInfo player) {
        }

        public override void Exit() {
        }
    }
}