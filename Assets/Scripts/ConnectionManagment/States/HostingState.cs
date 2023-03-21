
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class HostingState : ConnectionState {
        public override void Enter() {
            m_ConnectionManager.NetworkManager.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        public override void Exit() { }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            return;
        }


    }
}