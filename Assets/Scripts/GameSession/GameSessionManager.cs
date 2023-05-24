using System;
using System.Collections.Generic;
using Project.VoiceChatUtils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Project.GameSession {

    public struct GameSessionPlayer : INetworkSerializable, IEquatable<GameSessionPlayer> {
        public FixedString32Bytes ID;
        public FixedString32Bytes PlayerName;
        public uint AudioUserId;

        public GameSessionPlayer(string id, string playerName, uint audioUserId) {
            ID = id;
            PlayerName = playerName;
            AudioUserId = audioUserId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out ID);
                reader.ReadValueSafe(out PlayerName);
                reader.ReadValueSafe(out AudioUserId);
            }
            else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(ID);
                writer.WriteValueSafe(PlayerName);
                writer.WriteValueSafe(AudioUserId);
            }
        }

        public bool Equals(GameSessionPlayer other) {
            return ID == other.ID && PlayerName == other.PlayerName && AudioUserId == other.AudioUserId;
        }
    }

    public class GameSessionManager : NetworkBehaviour {
        public Action<GameSessionManager> changed;

        [Inject] AudioChannel m_AudioChannel;

        public NetworkList<GameSessionPlayer> playersNetworkList;
        string localPlayerId;

        public string LocalPlayerId => localPlayerId;

        public List<GameSessionPlayer> Players {
            get {
                Debug.Log("playersNetworkList" + playersNetworkList.Count);
                return new List<GameSessionPlayer>();
            }
        }

        List<GameSessionPlayer> _bufferedPlayers = new List<GameSessionPlayer>();

        void Awake() {
            playersNetworkList = new NetworkList<GameSessionPlayer>();
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            playersNetworkList.OnListChanged += OnPlayersListChanged;

            if (playersNetworkList.Count > 0) {
                SyncRemotePlayerListWithLocal(playersNetworkList);
            }

            if (_bufferedPlayers.Count != 0) { // syncing player list that was filled before spawn
                Debug.Log("_bufferedPlayers " + _bufferedPlayers.Count);
                foreach (var player in _bufferedPlayers) {
                    AddPlayer(player.ID.ToString(), player.PlayerName.ToString(), player.AudioUserId);
                }
            }
        }

        public void SetLocalPlayerId(string id) {
            localPlayerId = id;
        }

        public void AddPlayer(string playerId, string playerName, uint audioUserId) {
            if (!IsSpawned) {
                _bufferedPlayers.Add(new GameSessionPlayer(playerId, playerName, audioUserId)); // add this before the spawn, so when spawn happen - sync it with networkList
            }
            else {
                AddPlayerServerRpc(playerId, playerName, audioUserId);
            }
        }



        [ServerRpc(RequireOwnership = false)]
        void AddPlayerServerRpc(string playerId, string playerName, uint audioUserId) {
            var player = new GameSessionPlayer(playerId, playerName, audioUserId);
            playersNetworkList.Add(player);
        }

        [ServerRpc(RequireOwnership = false)]
        void RemovePlayerServerRpc(GameSessionPlayer player) {
            if (!playersNetworkList.Contains(player)) {
                Debug.LogError($"Error while removing player - Player with this id ({player.ID}) doesn't exist");
                return;
            }

            playersNetworkList.Remove(player);
        }

        void OnPlayersListChanged(NetworkListEvent<GameSessionPlayer> e) {
            Debug.Log("OnPlayersListChanged " + e.Type + "; " + playersNetworkList.Count);
            switch (e.Type) {
                case NetworkListEvent<GameSessionPlayer>.EventType.Add:
                    OnAddPlayer(e.Value);
                    break;
            }
        }

        public GameSessionPlayer? GetPlayerByAudioUserId(uint id) {
            foreach (var player in playersNetworkList) {
                if (player.AudioUserId == id) {
                    return player;
                }
            }
            return null;
        }

        void SyncRemotePlayerListWithLocal(NetworkList<GameSessionPlayer> remotePlayers) {
            foreach (var remotePlayer in remotePlayers) {
                OnAddPlayer(remotePlayer);
            }
        }

        void OnChanged() {
            changed?.Invoke(this);
        }
        void OnAddPlayer(GameSessionPlayer player) {
            m_AudioChannel.AddUser(player.AudioUserId, player.ID.ToString());

            OnChanged();
        }
    }
}