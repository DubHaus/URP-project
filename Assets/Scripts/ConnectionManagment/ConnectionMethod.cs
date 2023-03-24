using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Project.Utils;
using Unity.Netcode;

namespace Project.ConnectionManagment {

    public abstract class ConnectionMethod {
        protected ConnectionManager m_ConnectionManager;
        protected readonly string m_PlayerName;

        public abstract Task SetupHostConnectionAsync(uint characterHash);
        public abstract Task SetupClientConnectionAsync(string joinCode, uint characterHash);

        public ConnectionMethod(ConnectionManager connectionManager, string playerName) {
            m_ConnectionManager = connectionManager;
            m_PlayerName = playerName;
        }

        protected void SetConnectionPayload(string playerId, string playerName, uint characterHash) {
            var payload = JsonUtility.ToJson(
                new ConnectionPayload() {
                    playerId = playerId,
                    playerName = playerName,
                    characterHash = characterHash,
                }
            );

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }

        protected string GetPlayerId() {
            if (UnityServices.State != ServicesInitializationState.Initialized) {
                return ClientPrefabs.GetGuid();
            }

            return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : ClientPrefabs.GetGuid();
        }
    }

    public class RelayConnectionMethod : ConnectionMethod {

        public RelayConnectionMethod(ConnectionManager connectionManager, string playerName)
            : base(connectionManager, playerName) {
        }

        public override async Task SetupClientConnectionAsync(string joinCode, uint characterHash) {
            Debug.Log("Setting up Unity Relay client");

            SetConnectionPayload(GetPlayerId(), m_PlayerName, characterHash);

            Debug.Log($"Setting Unity Relay client with join code {joinCode}");

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var utp = (UnityTransport)m_ConnectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(joinAllocation, OnlineState.k_DtlsConnType));
        }

        public override async Task SetupHostConnectionAsync(uint characterHash) {
            Debug.Log("Setting up Unity Relay host");

            SetConnectionPayload(GetPlayerId(), m_PlayerName, characterHash);

            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(m_ConnectionManager.MaxConnectedPlayer, region: null);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            Debug.Log($"server: connection data: {hostAllocation.ConnectionData[0]} {hostAllocation.ConnectionData[1]}, " +
                $"allocation ID:{hostAllocation.AllocationId}, region:{hostAllocation.Region}");
            Debug.Log($"JOIN CODE: {joinCode}");
            m_ConnectionManager.joinCode = joinCode;


            var utp = (UnityTransport)m_ConnectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(hostAllocation, OnlineState.k_DtlsConnType));
        }

    }
}
