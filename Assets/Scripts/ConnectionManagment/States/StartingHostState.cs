
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class StartingHostState : ConnectionState {

        ConnectionMethod m_ConnectionMethod;
        uint characterHash;

        public StartingHostState Configure(ConnectionMethod connectionMethod, uint characterHash) {
            m_ConnectionMethod = connectionMethod;
            this.characterHash = characterHash;
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

            await m_ConnectionMethod.SetupHostConnectionAsync(characterHash);

            if (!m_ConnectionManager.NetworkManager.StartHost()) {
                Debug.LogError("Errror when starting host");
            }
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = connectionPayload.characterHash;
            response.Position = Vector3.zero;
            return;
        }
    }
}