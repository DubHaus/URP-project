using System;

namespace Project.UnityServices.Auth {


    public class LocalUserProfile {

        public Action<LocalUserProfile> changed;

        UserProfile m_UserProfile;

        public LocalUserProfile() {
            m_UserProfile = new UserProfile(null, false, null, null);
        }

        public struct UserProfile {
            public string PlayerName;
            public bool IsAuthorized;
            public string PlayerID;
            public DateTime? CreatedAt;

            public UserProfile(string playerName, bool isAuthorized, string playerID, DateTime? createdAt) {
                PlayerName = playerName;
                IsAuthorized = isAuthorized;
                PlayerID = playerID;
                CreatedAt = createdAt;
            }
        }

        public string PlayerName => m_UserProfile.PlayerName;
        public string PlayerID => m_UserProfile.PlayerID;
        public bool IsAuthorized => m_UserProfile.IsAuthorized;
        public DateTime? CreatedAt => m_UserProfile.CreatedAt;

        public void UpdateUserProfile(UserProfile profile) {
            m_UserProfile = profile;
            OnChanged();
        }

        void OnChanged() {
            changed?.Invoke(this);
        }
    }
}