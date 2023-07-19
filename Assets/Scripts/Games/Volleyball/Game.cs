namespace Games.Volleyball {
    public class Game {
        GameController m_GameController;
        PlayersManager m_PlayersManager;
        Ball m_Ball;

        public Game(GameController gameController, PlayersManager playersManager, Ball ball) {
            m_GameController = gameController;
            m_PlayersManager = playersManager;
            m_Ball = ball;
        }

        public void StartGame() {
            foreach (var player in m_PlayersManager.VolleyballPlayers) {
                player.punch += () => OnPunch(player);
            }
        }


        void OnPunch(VolleyballPlayer player) {
        }
        
        public void EndGame() {
            foreach (var player in m_PlayersManager.VolleyballPlayers) {
                player.punch -= () => OnPunch(player);
            }
        }
    }
}