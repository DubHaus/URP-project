using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.UnityServices.Infrastructure;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.UnityServices.Lobbies {
    public class LobbyServiceFacade : IDisposable, IStartable {

        [Inject] LifetimeScope m_ParentScope;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] LocalLobby m_LocalLobby;

        LifetimeScope m_ServiceScope;
        LobbyAPIInterface m_LobbyApiInterface;
        JoinedLobbyContentHeartbeat m_JoinedLobbyContentHeartbeat;
        LobbyEventConnectionState m_LobbyEventConnectionState = LobbyEventConnectionState.Unknown;

        RateLimitCooldown m_rateLimitHost;
        RateLimitCooldown m_RateLimitQuery;
        RateLimitCooldown m_RateLimitJoin;

        const float k_HeartbeatPeriod = 8; // The heartbeat must be rate-limited to 5 calls per 30 seconds. We'll aim for longer in case periods don't align.
        float m_HeartbeatTime = 0;


        bool m_IsTracking = false;

        ILobbyEvents m_lobbyEvents;
        public Lobby CurrentUnityLobby { get; private set; }

        public void Start() {
            m_ServiceScope = m_ParentScope.CreateChild(builder => {
                builder.Register<LobbyAPIInterface>(Lifetime.Singleton);
                builder.Register<JoinedLobbyContentHeartbeat>(Lifetime.Singleton);
            });
            m_LobbyApiInterface = m_ServiceScope.Container.Resolve<LobbyAPIInterface>();
            m_JoinedLobbyContentHeartbeat = m_ServiceScope.Container.Resolve<JoinedLobbyContentHeartbeat>();

            m_rateLimitHost = new RateLimitCooldown(3f);
            m_RateLimitQuery = new RateLimitCooldown(1f);
            m_RateLimitJoin = new RateLimitCooldown(3f);
        }

        public void Dispose() {
            EndTracking();
            if (m_ServiceScope != null) {
                m_ServiceScope.Dispose();
            }
        }

        public void SetRemoteLobby(Lobby lobby) {
            CurrentUnityLobby = lobby;
            m_LocalLobby.ApplyRemoteData(lobby);
        }

        public async Task<(bool Success, List<LocalLobby> result)> FetchLobbyList() {
            if (!m_RateLimitQuery.CanCall) {
                Debug.LogWarning("Retrieve Lobby list hit the rate limit. Will try again soon...");
                return (false, null);
            }

            try {
                var response = await m_LobbyApiInterface.QueryAllLobbies();
                Debug.Log("FetchLobbyList");
                Debug.Log(response);
                return (true, LocalLobby.CreateLocalLobbies(response));
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else {
                    Debug.LogError("Error while Fetching lobby list");
                    Debug.LogError(e);
                }
            }
            return (false, null);
        }


        public async Task<(bool Success, Lobby Lobby)> TryJoinLobbyAsync(string lobbyID, string lobbyCode) {
            if (!m_RateLimitJoin.CanCall ||
                (lobbyID == null && lobbyCode == null)) {
                Debug.LogWarning("Join Lobby hit the rate limit");
                return (false, null);
            }
            try {
                if (!string.IsNullOrEmpty(lobbyCode)) {
                    var lobby = await m_LobbyApiInterface.JoinLobbyByCode(
                        AuthenticationService.Instance.PlayerId,
                        lobbyCode,
                        m_LocalUser.GetDataForUnityServices()
                    );
                    return (true, lobby);
                }
                else {
                    var lobby = await m_LobbyApiInterface.JoinLobbyById(
                        AuthenticationService.Instance.PlayerId,
                        lobbyID,
                        m_LocalUser.GetDataForUnityServices()
                    );
                    return (true, lobby);
                }
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_RateLimitJoin.PutOnCooldown();
                }
                else {
                    Debug.LogError("Error while joining lobby");
                    Debug.LogError(e);
                }
            }

            return (false, null);
        }

        public async Task<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxPlayers, bool isPrivate) {
            if (!m_rateLimitHost.CanCall) {
                Debug.LogWarning("Create Lobby hit the rate limit.");
                return (false, null);
            }

            try {
                var lobby = await m_LobbyApiInterface.CreateLobby(
                    AuthenticationService.Instance.PlayerId,
                    lobbyName,
                    maxPlayers,
                    isPrivate,
                    m_LocalUser.GetDataForUnityServices(),
                    null
                );
                return (true, lobby);
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_rateLimitHost.PutOnCooldown();
                }
                else {
                    Debug.LogError(e.Message);
                }
            }

            return (false, null);
        }

        void ResetLobby() {
            CurrentUnityLobby = null;
            m_LocalUser.ResetState();
            m_LocalLobby?.Reset(m_LocalUser);
        }

        /// <summary>
        /// Attempt to leave a lobby
        /// </summary>
        public async void LeaveLobbyAsync() {
            string uasId = AuthenticationService.Instance.PlayerId;
            try {
                await m_LobbyApiInterface.RemovePlayerFromLobby(uasId, m_LocalLobby.LobbyID);
                ResetLobby();
            }
            catch (LobbyServiceException e) {
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost) {
                    Debug.LogError("Error while leaving lobby");
                    Debug.LogError(e);
                }
            }
        }

        public async void DeleteLobbyAsync() {
            if (m_LocalUser.IsHost) {
                try {
                    await m_LobbyApiInterface.DeleteLobby(m_LocalLobby.LobbyID);
                    ResetLobby();
                }
                catch (LobbyServiceException e) {
                    Debug.LogError("Error while deleting lobby");
                    Debug.LogError(e);
                }
            }
            else {
                Debug.LogError("Only the host can delete lobby.");
            }
        }

        public void BeginTracking() {
            if (!m_IsTracking) {
                m_IsTracking = true;
                SubscribeToJoinedLobbyAsync();
                m_JoinedLobbyContentHeartbeat.BeginTracking();
            }
        }
        public void EndTracking() {
            if (CurrentUnityLobby != null) {
                if (m_LocalUser.IsHost) {
                    DeleteLobbyAsync();
                }
                else {
                    LeaveLobbyAsync();
                }
            }

            if (m_IsTracking) {
                m_IsTracking = false;
                UnsubscribeToJoinedLobbyAsync();
            }
        }

        void OnLobbyChanged(ILobbyChanges changes) {
            if (changes.LobbyDeleted) {
                Debug.Log("Lobby deleted");
                ResetLobby();
            }
            else {
                Debug.Log("Lobby updated");
                changes.ApplyToLobby(CurrentUnityLobby);


                if (!m_LocalUser.IsHost) {
                    foreach (var user in m_LocalLobby.LobbyUsers) {
                        if (user.Value.IsHost) {
                            return;
                        }
                    }
                    Debug.LogWarning("Host left the lobby. Disconnecting.");
                    EndTracking();
                    // no need to disconnect Netcode, it should already be handled by Netcode's callback to disconnect
                }
            }
        }

        void OnKickedFromLobby() {
            Debug.Log("Kicked from lobby");
            ResetLobby();
        }

        void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state) {
            m_LobbyEventConnectionState = state;
            Debug.Log($"LobbyEventConnectionState changed to {state}");
        }

        async void SubscribeToJoinedLobbyAsync() {
            var lobbyEventsCallbacks = new LobbyEventCallbacks();
            lobbyEventsCallbacks.LobbyChanged += OnLobbyChanged;
            lobbyEventsCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventsCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

            m_lobbyEvents = await m_LobbyApiInterface.SubscribeToLobby(m_LocalLobby.LobbyID, lobbyEventsCallbacks);
            m_JoinedLobbyContentHeartbeat.BeginTracking();
        }

        async void UnsubscribeToJoinedLobbyAsync() {
            if (m_lobbyEvents != null && m_LobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed) {
                try {
                    await m_lobbyEvents.UnsubscribeAsync();
                }
                catch (ObjectDisposedException e) {
                    // This exception occurs in the editor when exiting play mode without first leaving the lobby.
                    // This is because Wire disposes of subscriptions internally when exiting play mode in the editor.
                    Debug.Log("Subscription is already disposed of, cannot unsubscribe.");
                    Debug.Log(e.Message);
                }
            }
            m_HeartbeatTime = 0;
            m_JoinedLobbyContentHeartbeat.EndTracking();
        }

        public async Task UpdatePlayerDataAsync(Dictionary<string, PlayerDataObject> data) {
            if (!m_RateLimitQuery.CanCall) {
                return;
            }

            try {
                var result = await m_LobbyApiInterface.UpdatePlayer(CurrentUnityLobby.Id, AuthenticationService.Instance.PlayerId, data, null, null);
                if (result != null) {
                    CurrentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost) // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                {
                    Debug.LogError("Error while Updating player data");
                    Debug.LogError(e);
                }
            }
        }

        public async Task UpdatePlayerRelayInfoAsync(string allocationId, string connectionInfo) {
            if (!m_RateLimitQuery.CanCall) {
                return;
            }

            try {
                await m_LobbyApiInterface.UpdatePlayer(
                    CurrentUnityLobby.Id,
                    AuthenticationService.Instance.PlayerId,
                    new Dictionary<string, PlayerDataObject>(),
                    allocationId,
                    connectionInfo
                );
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else {
                    Debug.LogError("Error while updating player Relay info");
                    Debug.LogError(e);
                }
                // TODO - retry logic? SDK is supposed to handle this eventually
            }
        }

        public async Task UpdateLobbyDataAsync(Dictionary<string, DataObject> data) {
            if (!m_rateLimitHost.CanCall) {
                return;
            }

            var dataCurr = CurrentUnityLobby.Data ?? new Dictionary<string, DataObject>();

            foreach (var dataNew in data) {
                if (dataCurr.ContainsKey(dataNew.Key)) {
                    dataCurr[dataNew.Key] = dataNew.Value;
                }
                else {
                    dataCurr.Add(dataNew.Key, dataNew.Value);
                }
            }

            var shouldLock = string.IsNullOrEmpty(m_LocalLobby.RelayJoinCode);

            try {
                var result = await m_LobbyApiInterface.UpdateLobby(CurrentUnityLobby.Id, dataCurr, shouldLock);
                if (result != null) {
                    CurrentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.RateLimited) {
                    m_rateLimitHost.PutOnCooldown();
                }
                else {
                    Debug.LogError("Error while updating lobby data");
                    Debug.LogError(e);
                }
            }
        }


        public void DoLobbyHeartbeat(float dt) {
            m_HeartbeatTime += dt;
            if (m_HeartbeatTime > k_HeartbeatPeriod) {
                m_HeartbeatTime -= k_HeartbeatPeriod;

                try {
                    m_LobbyApiInterface.SendHeartbeatPing(CurrentUnityLobby.Id);
                }
                catch (LobbyServiceException e) {
                    // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                    if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost) {
                        Debug.LogError(e);
                    }
                }
            }
        }
    }

}