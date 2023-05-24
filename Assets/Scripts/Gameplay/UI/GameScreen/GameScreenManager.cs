using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using Project.VoiceChatUtils;
using Project.GameSession;
using Unity.Netcode;

namespace Project.Gameplay.UI {

    public struct Player {
        public string name;

        public Player(string name) { this.name = name; }
    }

    public struct GameInfo {
        public string name;
        public string status;
        public int playersCount;
        public List<Player> players;

        public GameInfo(string name, string status, List<Player> players) {
            this.name = name;
            this.status = status;
            this.playersCount = players.Count;
            this.players = players;
        }
    }

    public class GameScreenManager : MonoBehaviour {
        GameSessionManager m_GameSessionManager;
        AudioChannel m_AudioChannel;

        Button toggleMicroButton;
        Button toggleAudioButton;
        Button showDetailsButton;
        UIDocument document;

        bool _showDetails;
        DetailsPanel detailsPanel;

        public bool showDetails {
            get => _showDetails;
            set {
                if (value != _showDetails) {
                    _showDetails = value;
                    ToggleDetailsPanel(value);
                }
            }
        }

        [Inject]
        void InjectDependenciesAndInitialize(GameSessionManager gameSessionManager, AudioChannel audioChannel) {
            m_GameSessionManager = gameSessionManager;
            m_AudioChannel = audioChannel;
        }

        public void Start() {
            document = GetComponent<UIDocument>();
            toggleMicroButton = document.rootVisualElement.Q<Button>("toggleMicro");
            toggleAudioButton = document.rootVisualElement.Q<Button>("toggleAudio");
            showDetailsButton = document.rootVisualElement.Q<Button>("showDetails");

            toggleMicroButton.clicked += ToggleMicro;
            toggleAudioButton.clicked += ToggleAudio;
            showDetailsButton.clicked += () => {
                showDetails = !showDetails;
            };
            m_GameSessionManager.changed += OnUpdateGameSession;
            m_AudioChannel.localUserChanged += UpdateLocalUserAudioUI;
            m_AudioChannel.userChanged += UpdateUserAudioUI;

            toggleMicroButton.Q(className: "icon").AddToClassList(m_AudioChannel.LocalMicroStatus == AudioDeviceStatus.Active ? "microIcon" : "microOffIcon");
            toggleAudioButton.Q(className: "icon").AddToClassList(m_AudioChannel.LocalAudioStatus == AudioDeviceStatus.Active ? "headsetIcon" : "headsetOffIcon");

            foreach (GameInfo game in GetGamesList()) {
                // Instantiate a template container.
                var elem = new GameCard(game.name, game.status, game.playersCount);
                // Add the custom element into the scene.
                document.rootVisualElement.Q("gameList").Add(elem);
                elem.SendToBack();
            }

            OnUpdateGameSession(m_GameSessionManager);
            UpdateLocalUserAudioUI(m_AudioChannel.LocalUser);
        }

        void ToggleDetailsPanel(bool showDetails) {
            if (showDetails) {
                detailsPanel = new DetailsPanel();
                document.rootVisualElement.Q("detailsPanel").Add(detailsPanel);
            }
            else {
                document.rootVisualElement.Q("detailsPanel").Remove(detailsPanel);
            }
        }

        void OnUpdateGameSession(GameSessionManager gameSession) {
            List<GameSessionPlayer> players = new List<GameSessionPlayer>();
            foreach (var player in gameSession.playersNetworkList) {
                if (player.ID != m_GameSessionManager.LocalPlayerId)
                    players.Add(player);
            }
            UpdatePlayersUI(players);
        }

        void UpdatePlayersUI(List<GameSessionPlayer> players) {
            document.rootVisualElement.Q("userAudioList").Clear();
            foreach (var player in players) {
                var audioUser = m_AudioChannel.GetUserByPlayerId(player.ID.ToString());
                var elem = new UserAudio(
                    player.PlayerName.ToString(),
                    $"user-audio-{audioUser.Value.ID}",
                    audioUser.Value.AudioStatus == AudioDeviceStatus.Active,
                    audioUser.Value.MicroStatus == AudioDeviceStatus.Active);
                // Add the custom element into the scene.
                document.rootVisualElement.Q("userAudioList").Add(elem);
            }
        }

        void UpdateLocalUserAudioUI(AudioChannel.AudioUser user) {
            if (user.MicroStatus == AudioDeviceStatus.Active) {
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

            if (user.AudioStatus == AudioDeviceStatus.Active) {
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

        void UpdateUserAudioUI(AudioChannel.AudioUser user) {
            Debug.Log("UpdateUserAudioUI " + user.MicroStatus + " " + user.ID);
            var userUI = document.rootVisualElement.Q<UserAudio>($"user-audio-{user.ID}");
            Debug.Log("UpdateUserAudioUI setUI " + userUI);
            userUI.SetAudio(user.AudioStatus == AudioDeviceStatus.Active);
            userUI.SetMicro(user.MicroStatus == AudioDeviceStatus.Active);
        }

        List<GameInfo> GetGamesList() {
            return new List<GameInfo> {
                new GameInfo("Volleyball", "11 : 9",
                    new List<Player>() {
                        new Player("Deaduser"),
                        new Player("Arties121"),
                        new Player("lancelot_de_luac"),
                    }
                ),
                new GameInfo("Chess", "Deaduser turn",
                    new List<Player>() {
                        new Player("Deaduser"),
                        new Player("Arties121"),
                    }
                ),
            };
        }

        void ToggleMicro() {
            m_AudioChannel.ToggleMicro();
        }

        void ToggleAudio() {
            m_AudioChannel.ToggleAudio();
        }
    }

}