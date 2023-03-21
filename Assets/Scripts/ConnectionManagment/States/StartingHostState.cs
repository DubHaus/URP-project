
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {
    public class StartingHostState : ConnectionState {

        ConnectionMethod m_ConnectionMethod;

        public StartingHostState Configure(ConnectionMethod connectionMethod) {
            m_ConnectionMethod = connectionMethod;
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

            await m_ConnectionMethod.SetupHostConnectionAsync();

            if (!m_ConnectionManager.NetworkManager.StartHost()) {
                Debug.LogError("Errror when starting host");
            }
        }
    }
}