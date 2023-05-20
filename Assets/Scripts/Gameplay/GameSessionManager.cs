using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Project.Gameplay {

    public struct GameSessionPlayer {
        public String ID;
        public String PlayerName;

        public GameSessionPlayer(string id, string playerName) {
            ID = id;
            PlayerName = playerName;
        }
    }

    struct GameSessionPlayerNV : INetworkSerializable, IEquatable<GameSessionPlayerNV> {
        public FixedString32Bytes ID;
        public FixedString32Bytes PlayerName;

        public GameSessionPlayerNV(string id, string playerName) {
            ID = id;
            PlayerName = playerName;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out ID);
                reader.ReadValueSafe(out PlayerName);
            }
            else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(ID);
                writer.WriteValueSafe(PlayerName);
            }
        }

        public bool Equals(GameSessionPlayerNV other) {
            return ID == other.ID && PlayerName == other.PlayerName;
        }
    }

    public class GameSessionManager : NetworkBehaviour {
        public Action<GameSessionManager> changed;
        NetworkList<GameSessionPlayerNV> playersNetworkList;

        List<GameSessionPlayer> players = new List<GameSessionPlayer>();
        public List<GameSessionPlayer> Players => players;

        void Awake() {
            playersNetworkList = new NetworkList<GameSessionPlayerNV>();
            playersNetworkList.OnListChanged += OnPlayersListChanged;
        }

        public void AddPlayer(GameSessionPlayer player) {
            AddPlayerServerRpc(new GameSessionPlayerNV(player.ID, player.PlayerName));
        }

        [ServerRpc(RequireOwnership = false)]
        void AddPlayerServerRpc(GameSessionPlayerNV player) {
            if (playersNetworkList.Contains(player)) {
                Debug.LogError($"Error while adding player - Player with this id ({player.ID}) already exist");
                return;
            }

            playersNetworkList.Add(player);
        }

        [ServerRpc(RequireOwnership = false)]
        void RemovePlayerServerRpc(GameSessionPlayerNV player) {
            if (!playersNetworkList.Contains(player)) {
                Debug.LogError($"Error while removing player - Player with this id ({player.ID}) doesn't exist");
                return;
            }

            playersNetworkList.Remove(player);
        }

        void OnPlayersListChanged(NetworkListEvent<GameSessionPlayerNV> _) {
            players = new List<GameSessionPlayer>();
            foreach (var player in playersNetworkList) {
                players.Add(new GameSessionPlayer(player.ID.ToString(), player.PlayerName.ToString()));
            }
            OnChanged();
        }

        void OnChanged() {
            changed?.Invoke(this);
        }
    }
}