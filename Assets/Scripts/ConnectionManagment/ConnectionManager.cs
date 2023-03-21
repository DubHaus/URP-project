using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Project.ConnectionManagment {

    public class ConnectionPayload {
        public string playerId;
        public string playerName;
        public bool isDebug;
    };

    public class ConnectionManager : MonoBehaviour {

        ConnectionState m_CurrentState;

        [Inject]
        IObjectResolver m_Resolver;

        [Inject]
        NetworkManager m_NetworkManager;
        public NetworkManager NetworkManager => m_NetworkManager;

        public int MaxConnectedPlayer = 8;

        internal readonly OfflineState m_OfflineState = new OfflineState();
        internal readonly ClientConnectingState m_ClientConnectingState = new ClientConnectingState();
        internal readonly ClientConnectedState m_ClientConnectedState = new ClientConnectedState();
        internal readonly StartingHostState m_StartingHostState = new StartingHostState();
        internal readonly HostingState m_HostingState = new HostingState();

        void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        void Start() {
            Debug.Log("START CONNECTION MANAGER");
            List<ConnectionState> states = new() { m_OfflineState, m_ClientConnectingState, m_ClientConnectedState, m_ClientConnectedState, m_HostingState, m_StartingHostState };
            foreach(var state in states) {
                m_Resolver.Inject(state);
            }

            m_CurrentState = m_OfflineState;

            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.OnTransportFailure += OnTransportFailure;
        }

        void OnDestroy() {
            NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.OnServerStarted -= OnServerStarted;
            NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.OnTransportFailure -= OnTransportFailure;
        }

        void OnClientConnectedCallback(ulong clientId) {
        }

        void OnClientDisconnectCallback(ulong clientId) {
        }

        void OnServerStarted() {
            m_CurrentState.OnServerStarted();
        }

        public void StartHost(string playerName) {
            m_CurrentState.StartHost(playerName);
        }

        public void StartClient(string playerName, string joinCode) {
            m_CurrentState.StartClient(playerName, joinCode);
        }

        public void ChangeState(ConnectionState nextState) {
            Debug.Log($"{name}: Changed connection state from {m_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

            if (m_CurrentState != null) {
                m_CurrentState.Exit();
            }
            m_CurrentState = nextState;
            nextState.Enter();
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest req,
            NetworkManager.ConnectionApprovalResponse res) {
            m_CurrentState.ApprovalCheck(req, res);
        }

        private void OnTransportFailure() {
            m_CurrentState.OnTransportFailure();
        }

    }
}
