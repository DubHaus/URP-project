using System;
using System.Collections;
using System.Collections.Generic;
using Project.UnityServices.Lobbies;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class StartingHostState : ConnectionState {

        ConnectionMethod m_ConnectionMethod;
        LocalLobby m_LocalLobby;

        public StartingHostState Configure(ConnectionMethod connectionMethod, LocalLobby localLobby) {
            m_ConnectionMethod = connectionMethod;
            m_LocalLobby = localLobby;
            return this;
        }

        public override void Enter() {
            StartHostFn();
        }

        public override void Exit() { }

        public override void OnServerStarted() {
            Debug.Log($"Server started");
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_HostingState);
        }

        async void StartHostFn() {
            Debug.Log($"Starting host");

            try {
                await m_ConnectionMethod.SetupHostConnectionAsync();
                Debug.Log($"Created relay allocation with join code {m_LocalLobby.RelayJoinCode}");
                if (!m_ConnectionManager.NetworkManager.StartHost()) {
                    StartHostFailed();
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
                StartHostFailed();
            }

        }

        void StartHostFailed() {
            Debug.LogError("Error while starting host");
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_OfflineState);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
            Debug.Log("connectionPayload: playerId: " + connectionPayload.playerId + "; playerName: " + connectionPayload.playerName);
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = Vector3.zero;
        }
    }
}