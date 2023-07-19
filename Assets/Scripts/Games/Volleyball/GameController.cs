using System.Collections.Generic;
using Project.Games.API;
using Project.Games.Volleyball;

namespace Games.Volleyball {
    public class GameController : GameControllerComponent {
        const string c_gameName = "Volleyball";
        public const int maxPlayers = 4;
        public const int minPlayers = 4;

        GameState activeState;
        GameInfoAPI m_GameInfoAPI;
        PlayersManager m_PlayersManager;
        Game m_Game;
        Ball m_Ball;

        List<PlayerInfo> players = new List<PlayerInfo>();


        internal readonly IdleState m_IdleState;
        internal readonly WaitingForPlayersState m_WaitingForPlayersState;
        internal readonly PlayingState m_PlayingState;

        public List<PlayerInfo> Players => players;

        public GameController() {
            m_PlayersManager = new PlayersManager(this);

            m_IdleState = new IdleState(this);
            m_WaitingForPlayersState = new WaitingForPlayersState(this);
            m_PlayingState = new PlayingState(this, m_PlayersManager);

            m_Ball = new Ball();
            m_Game = new Game(this, m_PlayersManager, m_Ball);

            activeState = m_IdleState;
        }

        public override void Start() {
            m_GameInfoAPI = new GameInfoAPI();
            m_GameInfoAPI.playerJoined += OnPlayerJoined;
        }

        public void SetGameState(GameState state) {
            activeState = state;
            OnChange();
        }

        public void StartGame() {
            m_Game.StartGame();
        }

        void AddPlayer(PlayerInfo player) {
            players.Add(player);
            OnChange();
        }

        void OnPlayerJoined(PlayerInfo player) {
            AddPlayer(player);
            activeState.PlayerJoined(player);
        }
        void OnChange() {
            m_GameInfoAPI.UpdateGameInfo(
                new GameInfo(c_gameName,
                    activeState.ToString(),
                    players.Count < maxPlayers && activeState.CanPlayerJoin
                )
            );
        }
    }
}