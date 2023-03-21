using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {

    public class OfflineState : ConnectionState {
        public override void Enter() {
            m_ConnectionManager.NetworkManager.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }

        public override void Exit() {
        }

        public override void StartHost(string playerName) {
            var method = new RelayConnectionMethod(m_ConnectionManager, playerName);
            m_ConnectionManager.ChangeState(
                m_ConnectionManager.m_StartingHostState.Configure(method)
                );

        }

        public override void StartClient(string playerName, string joinCode) {
            var method = new RelayConnectionMethod(m_ConnectionManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnectingState.Configute(method, joinCode));
        }
    }
}