
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class ClientConnectingState : ConnectionState {

        ConnectionMethod m_ConnectionMethod;
        string joinCode;

        public ClientConnectingState Configute(ConnectionMethod method, string joinCode) {
            m_ConnectionMethod = method;
            this.joinCode = joinCode;
            return this;
        }

        public override async void Enter() {
            try {
                await m_ConnectionMethod.SetupClientConnectionAsync(joinCode);
                if (!m_ConnectionManager.NetworkManager.StartClient()) {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception e) {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                // TODO add transition to failed state
                throw;
            }
        }

        public override void Exit() { }

        public override void OnClientConnected(ulong _) {
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnectedState);
        }
    }
}