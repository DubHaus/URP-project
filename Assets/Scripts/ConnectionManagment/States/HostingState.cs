
using System;
using System.Collections;
using System.Collections.Generic;
using Project.VoiceChatUtils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class HostingState : ConnectionState {
        public override void Enter() {
            m_ConnectionManager.NetworkManager.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            Debug.Log("JOIN CODE: " + m_ConnectionManager.joinCode);


            AgoraVoiceController.Instance.Join();
        }

        public override void OnClientConnected(ulong clientId)
        {
            Debug.Log("Client connected " + clientId);
        }

        public override void Exit() {
            AgoraVoiceController.Instance.Leave();
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
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