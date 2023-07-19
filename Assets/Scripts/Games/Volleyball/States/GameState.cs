using Games.Volleyball;
using Project.Games.API;

namespace Project.Games.Volleyball {
    public abstract class GameState {

        public abstract void Enter();
        public abstract void Exit();

        public abstract bool CanPlayerJoin {
            get;
        }

        public virtual void PlayerJoined(PlayerInfo player) { }
    }
}