using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Project.UnityServices.Lobbies {

    public class LocalLobbyUser {
        public event Action<LocalLobbyUser> changed;

        public LocalLobbyUser() {
            m_UserData = new UserData(false, null, null);
        }

        public struct UserData {
            public bool IsHost { get; set; }
            public string DisplayName { get; set; }
            public string ID { get; set; }

            public UserData(bool isHost, string displayName, string id) {
                IsHost = isHost;
                DisplayName = displayName;
                ID = id;
            }
        }

        UserData m_UserData;
        
        public void ResetState() {
            m_UserData = new UserData(false, m_UserData.DisplayName, m_UserData.ID);
        }

        public bool IsHost {
            get { return m_UserData.IsHost; }
            set {
                if (m_UserData.IsHost != value) {
                    m_UserData.IsHost = value;
                    OnChanged();
                }
            }
        }

        public string DisplayName {
            get { return m_UserData.DisplayName; }
            set {
                if (m_UserData.DisplayName != value) {
                    m_UserData.DisplayName = value;
                    OnChanged();
                }
            }
        }

        public string ID {
            get { return m_UserData.ID; }
            set {
                if (m_UserData.ID != value) {
                    m_UserData.ID = value;
                    OnChanged();
                }
            }
        }

        public void CopyDataFrom(LocalLobbyUser lobby) {
            var data = lobby.m_UserData;
            bool lastChanged =
                (m_UserData.IsHost != data.IsHost) ||
                (m_UserData.DisplayName != data.DisplayName) ||
                (m_UserData.ID != data.ID);
            if (!lastChanged) {
                return;
            }

            m_UserData = data;
            OnChanged();
        }


        void OnChanged() {
            changed?.Invoke(this);
        }

        public Dictionary<string, PlayerDataObject> GetDataForUnityServices() =>
            new Dictionary<string, PlayerDataObject> {
                { "DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DisplayName) }
            };

    }
}