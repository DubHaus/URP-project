using System;
using System.Collections;
using System.Collections.Generic;
using Project.Gameplay;
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
        public override void Enter() {
            m_ConnectionManager.NetworkManager.SceneManager.LoadScene("GLDScene", LoadSceneMode.Single);
            //m_ConnectionManager.NetworkManager.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            AgoraVoiceController.Instance.Join();
        }

        public override void OnClientConnected(ulong clientId) {
            Debug.Log("Client connected " + clientId);
        }

        public override void Exit() {
            AgoraVoiceController.Instance.Leave();
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
            
            m_GameSessionManager.AddPlayer(new GameSessionPlayer(connectionPayload.playerId, connectionPayload.playerName));
            
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = new Vector3(0, 3, 0);
        }
    }
}