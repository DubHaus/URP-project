using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Games.Volleyball {
    public class PlayersManager {
        GameController m_GameController;

        Dictionary<string, VolleyballPlayer> volleyballPlayers;

        public List<VolleyballPlayer> VolleyballPlayers => volleyballPlayers.Values.ToList();
        public PlayersManager(GameController gameController) {
            m_GameController = gameController;
        }

        public void InitPlayers() {
            int idx = 0;
            foreach (var player in m_GameController.Players) {
                var volleyballPlayer = new VolleyballPlayer(player.PlayerID);
                volleyballPlayers.Add(player.PlayerID, volleyballPlayer);
            }
        }
    }
}