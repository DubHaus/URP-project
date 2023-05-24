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

namespace Project.Gameplay.UI {
    public class MainScreenManager : MonoBehaviour {

        AuthenticationServiceFacade m_authenticationServiceFacade;
        LobbyServiceFacade m_LobbyServiceFacade;
        ConnectionManager m_ConnectionManager;
        LocalLobby m_LocalLobby;
        LocalLobbyUser m_LocalUser;
        LocalUserProfile m_LocalUserProfile;
        AudioChannel m_AudioChannel;
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
            AudioChannel audioChannel,
            UpdateRunner updateRunner) {
            m_authenticationServiceFacade = authenticationServiceFacade;
            m_LobbyServiceFacade = lobbyServiceFacade;
            m_ConnectionManager = connectionManager;
            m_LocalLobby = localLobby;
            m_LocalUser = lobbyUser;
            m_LocalUserProfile = localUserProfile;
            m_AudioChannel = audioChannel;
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

            toggleMicroButton.Q(className: "icon").AddToClassList(m_AudioChannel.LocalMicroStatus == AudioDeviceStatus.Active ? "microIcon" : "microOffIcon");
            toggleAudioButton.Q(className: "icon").AddToClassList(m_AudioChannel.LocalAudioStatus == AudioDeviceStatus.Active ? "headsetIcon" : "headsetOffIcon");
            
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

        private void ShowConnectByCodeModal() {
            ShowModal(new ConnectByCodeModal(ConnectByCode, HideModal));
        }

        private void ShowCreateServerModal() {
            Debug.Log("ShowCreateServerModal");
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
            m_AudioChannel.ToggleMicro();
            if (m_AudioChannel.LocalMicroStatus == AudioDeviceStatus.Active) {
                toggleMicroButton.Q(className: "icon")
                    .RemoveFromClassList("microOffIcon");
                toggleMicroButton.Q(className: "icon")
                    .AddToClassList("microIcon");
            }
            else {
                toggleMicroButton.Q(className: "icon")
                    .RemoveFromClassList("microIcon");
                toggleMicroButton.Q(className: "icon")
                    .AddToClassList("microOffIcon");
            }
        }

        private void ToggleAudio() {
            m_AudioChannel.ToggleAudio();

            if (m_AudioChannel.LocalAudioStatus == AudioDeviceStatus.Active) {
                toggleAudioButton.Q(className: "icon")
                    .RemoveFromClassList("headsetOffIcon");
                toggleAudioButton.Q(className: "icon")
                    .AddToClassList("headsetIcon");
            }
            else {
                toggleAudioButton.Q(className: "icon")
                    .RemoveFromClassList("headsetIcon");
                toggleAudioButton.Q(className: "icon")
                    .AddToClassList("headsetOffIcon");
            }
        }
    }

}