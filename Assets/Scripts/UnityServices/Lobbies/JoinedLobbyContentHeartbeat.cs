using Project.Infrastructure;
using VContainer;

namespace Project.UnityServices.Lobbies {
    public class JoinedLobbyContentHeartbeat {
        [Inject] LocalLobby m_LocalLobby;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] UpdateRunner m_UpdateRunner;
        [Inject] LobbyServiceFacade m_LobbyServiceFacade;

        int m_AwaitingQueryCount = 0;
        bool m_ShouldPushData = false;

        public void BeginTracking() {
            m_UpdateRunner.Subscribe(OnUpdate, 1.5f);
            m_LocalLobby.changed += OnLocalLobbyChanged;
        }

        async void OnUpdate(float period) {
            if (m_AwaitingQueryCount > 0) {
                return;
            }

            if (m_LocalUser.IsHost) {
                m_LobbyServiceFacade.DoLobbyHeartbeat(period);
            }

            if (m_ShouldPushData) {
                m_ShouldPushData = false;

                if (m_LocalUser.IsHost) {
                    m_AwaitingQueryCount++; // todo this should disappear once we use await correctly. This causes issues at the moment if OnSuccess isn't called properly
                    await m_LobbyServiceFacade.UpdateLobbyDataAsync(m_LocalLobby.GetDataForUnityServices());
                    m_AwaitingQueryCount--;
                }
                m_AwaitingQueryCount++;
                await m_LobbyServiceFacade.UpdatePlayerDataAsync(m_LocalUser.GetDataForUnityServices());
                m_AwaitingQueryCount--;
            }
        }

        void OnLocalLobbyChanged(LocalLobby lobby) {
            if (string.IsNullOrEmpty(lobby.LobbyID)) {
                EndTracking();
            }

            m_ShouldPushData = true;
        }

        public void EndTracking() {
            m_ShouldPushData = false;
            m_UpdateRunner.Unsubscribe((OnUpdate));
            m_LocalLobby.changed -= OnLocalLobbyChanged;
        }
    }
}