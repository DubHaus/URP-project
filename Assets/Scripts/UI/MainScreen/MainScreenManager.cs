using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Project.ConnectionManagment;
using Project.Infrastructure;
using VContainer;
using Project.VoiceChatUtils;
using Project.UnityServices.Auth;
using Project.UnityServices.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Project.UI {

    public struct ServerInfo {
        public string name;
        public DateTime lastVisit;
        public Texture2D image;
        public Color mainColor;
        public Color secondaryColor;

        public ServerInfo(string name, DateTime lastVisit, Texture2D image, Color mainColor, Color secondaryColor) {
            this.name = name;
            this.lastVisit = lastVisit;
            this.image = image;
            this.mainColor = mainColor;
            this.secondaryColor = secondaryColor;
        }
    }

    public class MainScreenManager : MonoBehaviour {
        [SerializeField] private Texture2D logoImage;

        AuthenticationServiceFacade m_authenticationServiceFacade;
        LobbyServiceFacade m_LobbyServiceFacade;
        ConnectionManager m_ConnectionManager;
        LocalLobby m_LocalLobby;
        LocalLobbyUser m_LocalUser;
        LocalUserProfile m_LocalUserProfile;
        UpdateRunner m_UpdateRunner;

        Button connectByCodeButton;
        Button createServerButton;
        Button toggleAudioButton;
        Button toggleMicroButton;
        UIDocument document;

        VisualElement activeModal;
        Label userName;

        [Inject]
        void InjectDependenciesAndInitialize(
            AuthenticationServiceFacade authenticationServiceFacade,
            LobbyServiceFacade lobbyServiceFacade,
            ConnectionManager connectionManager,
            LocalLobby localLobby,
            LocalLobbyUser lobbyUser,
            LocalUserProfile localUserProfile,
            UpdateRunner updateRunner) {
            m_authenticationServiceFacade = authenticationServiceFacade;
            m_LobbyServiceFacade = lobbyServiceFacade;
            m_ConnectionManager = connectionManager;
            m_LocalLobby = localLobby;
            m_LocalUser = lobbyUser;
            m_LocalUserProfile = localUserProfile;
            m_UpdateRunner = updateRunner;
        }

        public void Start() {
            document = GetComponent<UIDocument>();
            connectByCodeButton = document.rootVisualElement.Q<Button>("connectByCodeButton");
            createServerButton = document.rootVisualElement.Q<Button>("createServerButton");
            toggleAudioButton = document.rootVisualElement.Q<Button>("audioButton");
            toggleMicroButton = document.rootVisualElement.Q<Button>("microButton");
            userName = document.rootVisualElement.Q<Label>("username");

            connectByCodeButton.clicked += ShowConnectByCodeModal;
            createServerButton.clicked += ShowCreateServerModal;
            toggleAudioButton.clicked += ToggleAudio;
            toggleMicroButton.clicked += ToggleMicro;

            m_LocalUserProfile.changed += UpdateUserProfile;

            m_UpdateRunner.Subscribe(PeriodicRefresh, 10f);

            toggleMicroButton.Q(className: "icon").AddToClassList(AgoraVoiceController.Instance.isMicroMuted ? "microOffIcon" : "microIcon");
            toggleAudioButton.Q(className: "icon").AddToClassList(AgoraVoiceController.Instance.isAudioMuted ? "headsetOffIcon" : "headsetIcon");

            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("CardElement");
            UpdateUserProfile(m_LocalUserProfile);
        }

        public void OnDestroy() {
            connectByCodeButton.clicked -= ShowConnectByCodeModal;
            createServerButton.clicked -= ShowCreateServerModal;
            toggleAudioButton.clicked -= ToggleAudio;
            toggleMicroButton.clicked -= ToggleMicro;

            m_LocalUserProfile.changed -= UpdateUserProfile;
            m_UpdateRunner.Unsubscribe(PeriodicRefresh);

        }

        private void SomeInteraction(ClickEvent evt) {
            // Interact with the elements here.
        }

        private List<ServerInfo> GetServerList() {
            return new List<ServerInfo> {
                new ServerInfo("Viva La Dirt League", new DateTime(), logoImage, Color.blue, Color.white),
                new ServerInfo("Champions Club", new DateTime(), logoImage, Color.red, Color.white)
            };
        }

        private void ShowConnectByCodeModal() {
            ShowModal(new ConnectByCodeModal(ConnectByCode, HideModal));
        }

        private void ShowCreateServerModal() {
            ShowModal(new CreateServerModal(CreateServer, HideModal));
        }

        void UpdateUserProfile(LocalUserProfile user) {
            userName.text = user.PlayerName;
        }

        async void PeriodicRefresh(float _) {
            var result = await m_LobbyServiceFacade.FetchLobbyList();
            if (result.Success) {
                UpdateServersUI(result.result);
            }
        }

        void UpdateServersUI(List<LocalLobby> list) {
            document.rootVisualElement.Q("lastVisitedServers").Clear();
            foreach (LocalLobby lobby in list) {
                // Instantiate a template container.
                var cardElement = new ServerCard(lobby.LobbyName);
                cardElement.RegisterCallback<ClickEvent>((_ => {
                    JoinLobbyRequest(lobby);
                }));
                // Add the custom element into the scene.
                document.rootVisualElement.Q("lastVisitedServers").Add(cardElement);
            }
        }

        async void JoinLobbyRequest(LocalLobby lobby) {
            var result = await m_LobbyServiceFacade.TryJoinLobbyAsync(lobby.LobbyID, lobby.LobbyCode);

            if (result.Success) {
                OnJoinedLobby(result.Lobby);
            }
        }

        void OnJoinedLobby(Lobby lobby) {
            m_LobbyServiceFacade.SetRemoteLobby(lobby);
            Debug.Log($"Joined lobby with code: {m_LocalLobby.LobbyCode}, Internal Relay Join Code{m_LocalLobby.RelayJoinCode}");
            m_ConnectionManager.StartClientLobby(m_LocalUser.DisplayName);
        }

        void ShowModal(VisualElement modal) {
            activeModal = modal;
            document.rootVisualElement.Add(activeModal);
            modal.style.backgroundColor = new Color(0, 0, 0, 0.2f);
            modal.style.height = 2000;
            modal.style.width = new StyleLength(Length.Percent(100));
            modal.style.position = Position.Absolute;
            modal.style.left = 0;
            modal.style.top = 0;
        }


        private void HideModal() {
            document.rootVisualElement.Remove(activeModal);
            activeModal = null;
        }

        private void ConnectByCode(string code) {
            ConnectionManager.Instance.StartClient("DeadUser", code, 11);
        }

        private async void CreateServer(string serverName, bool isPrivate) {
            var lobbyCreationAttempt = await m_LobbyServiceFacade.TryCreateLobbyAsync(serverName, m_ConnectionManager.MaxConnectedPlayer, isPrivate);
            if (lobbyCreationAttempt.Success) {
                m_LocalUser.IsHost = true;
                m_LobbyServiceFacade.SetRemoteLobby(lobbyCreationAttempt.Lobby);

                Debug.Log($"Created lobby with ID: {m_LocalLobby.LobbyID} and code {m_LocalLobby.LobbyCode}");
                m_ConnectionManager.StartHostLobby(m_LocalUser.DisplayName);
            }
        }

        void ToggleMicro() {
            AgoraVoiceController.Instance.PresetToggleMicro();
            if (AgoraVoiceController.Instance.isMicroMuted) {
                toggleMicroButton.Q(className: "icon")
                    .RemoveFromClassList("microIcon");
                toggleMicroButton.Q(className: "icon")
                    .AddToClassList("microOffIcon");
            }
            else {
                toggleMicroButton.Q(className: "icon")
                    .RemoveFromClassList("microOffIcon");
                toggleMicroButton.Q(className: "icon")
                    .AddToClassList("microIcon");
            }
        }

        private void ToggleAudio() {
            AgoraVoiceController.Instance.PresetToggleAudio();

            if (AgoraVoiceController.Instance.isAudioMuted) {
                toggleAudioButton.Q(className: "icon")
                    .RemoveFromClassList("headsetIcon");
                toggleAudioButton.Q(className: "icon")
                    .AddToClassList("headsetOffIcon");
            }
            else {
                toggleAudioButton.Q(className: "icon")
                    .RemoveFromClassList("headsetOffIcon");
                toggleAudioButton.Q(className: "icon")
                    .AddToClassList("headsetIcon");
            }
        }
    }

}