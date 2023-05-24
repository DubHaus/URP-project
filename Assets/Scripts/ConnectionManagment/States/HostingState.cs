using System;
using System.Collections;
using System.Collections.Generic;
using Project.GameSession;
using Project.UnityServices.Lobbies;
using Project.VoiceChatUtils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Project.ConnectionManagment {
    public class HostingState : ConnectionState {

        [Inject] LocalLobby m_LocalLobby;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] GameSessionManager m_GameSessionManager;
        [Inject] AudioChannel m_AudioChannel;
        public override void Enter() {
            m_ConnectionManager.NetworkManager.SceneManager.LoadScene("GLDScene", LoadSceneMode.Single);
            m_AudioChannel.JoinChannel();
        }

        public override void OnClientConnected(ulong clientId) {
            Debug.Log("Client connected " + clientId);
        }

        public override void Exit() {
            m_AudioChannel.LeaveChannel();
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

            m_GameSessionManager.AddPlayer(connectionPayload.playerId, connectionPayload.playerName, connectionPayload.audioId);

            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = new Vector3(0, 3, 0);
        }
    }
}