using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project.UnityServices.Lobbies {
    public sealed class LocalLobby {
        public event Action<LocalLobby> changed;

        LobbyData m_Data;

        public static List<LocalLobby> CreateLocalLobbies(QueryResponse response) {
            var list = new List<LocalLobby>();
            foreach (var lobby in response.Results) {
                list.Add(Create(lobby));
            }
            return list;
        }


        public static LocalLobby Create(Lobby lobby) {
            var data = new LocalLobby();
            data.ApplyRemoteData(lobby);
            return data;
        }

        public string RelayJoinCode {
            get => m_Data.RelayJoinCode;
            set {
                m_Data.RelayJoinCode = value;
                OnChanged();
            }
        }

        public struct LobbyData {
            public string LobbyID { get; set; }
            public string LobbyCode { get; set; }
            public string RelayJoinCode { get; set; }
            public string LobbyName { get; set; }
            public bool Private { get; set; }
            public int MaxPlayerCount { get; set; }

            public LobbyData(LobbyData existing) {
                LobbyID = existing.LobbyID;
                LobbyCode = existing.LobbyCode;
                RelayJoinCode = existing.RelayJoinCode;
                LobbyName = existing.LobbyName;
                Private = existing.Private;
                MaxPlayerCount = existing.MaxPlayerCount;
            }

            public LobbyData(string lobbyCode) {
                LobbyID = null;
                LobbyCode = lobbyCode;
                RelayJoinCode = null;
                LobbyName = null;
                Private = false;
                MaxPlayerCount = -1;
            }
        }

        Dictionary<string, LocalLobbyUser> m_LobbyUsers = new Dictionary<string, LocalLobbyUser>();
        public Dictionary<string, LocalLobbyUser> LobbyUsers => m_LobbyUsers;

        public void AddUser(LocalLobbyUser user) {
            DoAddUser(user);
            OnChanged();
        }

        void DoAddUser(LocalLobbyUser user) {
            m_LobbyUsers.Add(user.ID, user);
            user.changed += OnChangedUser;
        }

        public void RemoveUser(LocalLobbyUser user) {
            DoRemoveUser(user);
            OnChanged();
        }

        void DoRemoveUser(LocalLobbyUser user) {
            if (!m_LobbyUsers.ContainsKey(user.ID)) {
                Debug.LogWarning($"Player {user.DisplayName} ({user.ID}) does not exist in the lobby: {LobbyID}");
                return;
            }
            m_LobbyUsers.Remove(user.ID);
            user.changed -= OnChangedUser;
        }

        void OnChangedUser(LocalLobbyUser user) {
            OnChanged();
        }

        void OnChanged() {
            changed?.Invoke(this);
        }

        public string LobbyID {
            get => m_Data.LobbyID;
            set {
                m_Data.LobbyID = value;
                OnChanged();
            }
        }

        public string LobbyCode {
            get => m_Data.LobbyCode;
            set {
                m_Data.LobbyCode = value;
                OnChanged();
            }
        }

        public string LobbyName {
            get => m_Data.LobbyName;
            set {
                m_Data.LobbyName = value;
                OnChanged();
            }
        }

        public void CopyDataFrom(LobbyData data, Dictionary<string, LocalLobbyUser> currUsers) {
            m_Data = data;
            if (currUsers == null) {
                m_LobbyUsers = new Dictionary<string, LocalLobbyUser>();
            }
            else {
                List<LocalLobbyUser> toRemove = new List<LocalLobbyUser>();
                foreach (var oldUser in m_LobbyUsers) {
                    if (currUsers.ContainsKey(oldUser.Key)) {
                        oldUser.Value.CopyDataFrom(currUsers[oldUser.Key]);
                    }
                    else {
                        toRemove.Add(oldUser.Value);
                    }
                }
                foreach (var user in toRemove) {
                    DoRemoveUser(user);
                }

                foreach (var user in currUsers) {
                    if (!m_LobbyUsers.ContainsKey(user.Key)) {
                        DoAddUser(user.Value);
                    }
                }
            }
            OnChanged();
        }

        public void ApplyRemoteData(Lobby lobby) {
            var info = new LobbyData();
            info.LobbyID = lobby.Id;
            info.LobbyName = lobby.Name;
            info.LobbyCode = lobby.LobbyCode;
            info.Private = lobby.IsPrivate;
            info.MaxPlayerCount = lobby.MaxPlayers;

            if (lobby.Data != null) {
                info.RelayJoinCode = lobby.Data.ContainsKey("RelayJoinCode") ? lobby.Data["RelayJoinCode"].Value : null;
            }
            else {
                info.RelayJoinCode = null;
            }
            var lobbyUsers = new Dictionary<string, LocalLobbyUser>();
            foreach (var player in lobby.Players) {
                if (player.Data != null) {
                    if (LobbyUsers.ContainsKey(player.Id)) {
                        lobbyUsers.Add(player.Id, LobbyUsers[player.Id]);
                        continue;
                    }
                }

                var incomingData = new LocalLobbyUser() {
                    IsHost = lobby.HostId.Equals(player.Id),
                    DisplayName = player.Data?.ContainsKey("DisplayName") == true ? player.Data["DisplayName"].Value : default,
                    ID = player.Id,
                };

                lobbyUsers.Add(incomingData.ID, incomingData);
            }

            CopyDataFrom(info, lobbyUsers);
        }

        public Dictionary<string, DataObject> GetDataForUnityServices() =>
            new Dictionary<string, DataObject>() {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, RelayJoinCode) }
            };

        public void Reset(LocalLobbyUser localUser) {
            CopyDataFrom(new LobbyData(), new Dictionary<string, LocalLobbyUser>());
            AddUser(localUser);
        }
    }
}