using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Project.Utils;
using Project.UnityServices.Lobbies;
using Project.VoiceChatUtils;

namespace Project.ConnectionManagment {

    public abstract class ConnectionMethod {
        protected ConnectionManager m_ConnectionManager;
        protected readonly string m_PlayerName;
        public ConnectionPayload payload;

        public abstract Task SetupHostConnectionAsync();
        public abstract Task SetupClientConnectionAsync();

        public ConnectionMethod(ConnectionManager connectionManager, string playerName) {
            m_ConnectionManager = connectionManager;
            m_PlayerName = playerName;
        }

        protected void SetConnectionPayload(string playerId, uint audioId, string playerName) {
            this.payload = new ConnectionPayload() {
                playerId = playerId,
                playerName = playerName,
                audioId = audioId,
            };

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    this.payload
                ));

            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }

        protected string GetPlayerId() {
            // if (Unity.Services.Core.UnityServices.State != ServicesInitializationState.Initialized) {
            //     return ClientPrefabs.GetGuid() + m_ProfileManager.Profile;
            // }
            if (!AuthenticationService.Instance.IsSignedIn) {
                Debug.LogError("Users is not authorized!");
            }
            return AuthenticationService.Instance.PlayerId;
            // return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : ClientPrefabs.GetGuid();
        }
    }

    public class RelayConnectionMethod : ConnectionMethod {

        LocalLobby m_LocalLobby;
        LobbyServiceFacade m_LobbyServiceFacade;
        AudioChannel m_AudioChannel;

        public RelayConnectionMethod(LocalLobby localLobby, LobbyServiceFacade lobbyServiceFacade, ConnectionManager connectionManager, AudioChannel audioChannel, string playerName)
            : base(connectionManager, playerName) {
            m_LocalLobby = localLobby;
            m_LobbyServiceFacade = lobbyServiceFacade;
            m_AudioChannel = audioChannel;
        }

        public override async Task SetupClientConnectionAsync() {
            Debug.Log("Setting up Unity Relay client");
            Debug.Log("GetPlayerId " + GetPlayerId());
            SetConnectionPayload(GetPlayerId(), m_AudioChannel.LocalUser.ID, m_PlayerName);

            Debug.Log($"Setting Unity Relay client with join code {m_LocalLobby.RelayJoinCode}");

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(m_LocalLobby.RelayJoinCode);
            await m_LobbyServiceFacade.UpdatePlayerRelayInfoAsync(joinAllocation.AllocationId.ToString(), m_LocalLobby.RelayJoinCode);

            var utp = (UnityTransport)m_ConnectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(joinAllocation, OnlineState.k_DtlsConnType));
        }

        public override async Task SetupHostConnectionAsync() {
            Debug.Log("Setting up Unity Relay host");
            Debug.Log("GetPlayerId " + GetPlayerId());
            SetConnectionPayload(GetPlayerId(), m_AudioChannel.LocalUser.ID, m_PlayerName);

            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(m_ConnectionManager.MaxConnectedPlayer, region: null);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

            Debug.Log($"server: connection data: {hostAllocation.ConnectionData[0]} {hostAllocation.ConnectionData[1]}, " +
                $"allocation ID:{hostAllocation.AllocationId}, region:{hostAllocation.Region}");
            Debug.Log($"JOIN CODE: {joinCode}");

            m_LocalLobby.RelayJoinCode = joinCode;
            await m_LobbyServiceFacade.UpdateLobbyDataAsync(m_LocalLobby.GetDataForUnityServices());
            await m_LobbyServiceFacade.UpdatePlayerRelayInfoAsync(hostAllocation.AllocationId.ToString(), joinCode);

            var utp = (UnityTransport)m_ConnectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(hostAllocation, OnlineState.k_DtlsConnType));
        }

    }
}