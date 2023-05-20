using System.Collections;
using System.Collections.Generic;
using Project.UnityServices.Lobbies;
using Project.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Project.ConnectionManagment {

    public class OfflineState : ConnectionState {

        [Inject] LocalLobby m_LocalLobby;
        [Inject] LobbyServiceFacade m_LobbyServiceFacade;
        [Inject] ProfileManager m_ProfileManager;

        public override void Enter() {
            m_ConnectionManager.NetworkManager.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }

        public override void Exit() {
        }

        // public override void StartHost(string playerName, uint characterHash) {
        //     var method = new RelayConnectionMethod(m_LocalLobby, m_LobbyServiceFacade, m_ConnectionManager, playerName);
        //     m_ConnectionManager.ChangeState(
        //         m_ConnectionManager.m_StartingHostState.Configure(method, characterHash)
        //     );
        //
        // }

        // public override void StartClient(string playerName, string joinCode, uint characterHash) {
        //     var method = new RelayConnectionMethod(m_LocalLobby, m_LobbyServiceFacade, m_ConnectionManager, playerName);
        //     m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnectingState.Configure(method, joinCode, characterHash));
        // }

        public override void StartHostLobby(string playerName) {
            var method = new RelayConnectionMethod(m_LocalLobby, m_LobbyServiceFacade, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHostState.Configure(method, m_LocalLobby));
        }

        public override void StartClientLobby(string playerName) {
            var method = new RelayConnectionMethod(m_LocalLobby, m_LobbyServiceFacade, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnectingState.Configure(method));
        }
    }
}