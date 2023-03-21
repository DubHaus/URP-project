using System;
using Unity.Netcode;
using VContainer;

namespace Project.ConnectionManagment {

    abstract public class ConnectionState {

        [Inject]
        protected ConnectionManager m_ConnectionManager;

        public abstract void Enter();
        public abstract void Exit();
        public virtual void StartClient(string playerName, string joinCode) { }
        public virtual void StartHost(string playerName) { }
        public virtual void OnClientConnected(ulong clientId) { }
        public virtual void OnServerStarted() { }

        public virtual void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response) {

        }

        public virtual void OnTransportFailure() { }

    }
}