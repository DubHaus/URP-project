using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Project.ConnectionManagment;
using Project.UnityServices.Lobbies;
using VContainer;
using Project.VoiceChatUtils;
using Project.Gameplay;

namespace Project.UI {

    public struct UserAudioInfo {
        public string name;
        public bool microOn;
        public bool audioOn;

        public UserAudioInfo(string name, bool microOn, bool audioOn) {
            this.name = name;
            this.microOn = microOn;
            this.audioOn = audioOn;
        }
    }

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
        LocalLobby m_LocalLobby;

        private Button toggleMicroButton;
        private Button toggleAudioButton;
        private Button showDetailsButton;
        private UIDocument document;

        private bool _showDetails = false;
        private DetailsPanel detailsPanel;

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
        void InjectDependenciesAndInitialize(LocalLobby localLobby) {
            m_LocalLobby = localLobby;
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

            toggleMicroButton.Q(className: "icon").AddToClassList(AgoraVoiceController.Instance.isMicroMuted ? "microOffIcon" : "microIcon");
            toggleAudioButton.Q(className: "icon").AddToClassList(AgoraVoiceController.Instance.isAudioMuted ? "headsetOffIcon" : "headsetIcon");

            foreach (GameInfo game in GetGamesList()) {
                // Instantiate a template container.
                var elem = new GameCard(game.name, game.status, game.playersCount);
                // Add the custom element into the scene.
                document.rootVisualElement.Q("gameList").Add(elem);
                elem.SendToBack();
            }
        }

        private void ToggleDetailsPanel(bool showDetails) {
            if (showDetails) {
                detailsPanel = new DetailsPanel();
                document.rootVisualElement.Q("detailsPanel").Add(detailsPanel);
            }
            else {
                document.rootVisualElement.Q("detailsPanel").Remove(detailsPanel);
            }
        }

        private List<UserAudioInfo> GetUsersList() {
            return new List<UserAudioInfo> {
                new UserAudioInfo("User123", true, true),
                new UserAudioInfo("lancelot_de_luac", true, false),
                new UserAudioInfo("moder_user", false, true),
                new UserAudioInfo("TTW_LGA_a2", false, false),
            };
        }

        public void UpdatePlayersUI(List<GameSessionPlayer> players) {
            document.rootVisualElement.Q("userAudioList").Clear();
            foreach (var player in players) {
                var elem = new UserAudio(player.PlayerName, false, false);
                // Add the custom element into the scene.
                document.rootVisualElement.Q("userAudioList").Add(elem);
            }
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
            AgoraVoiceController.Instance.ToggleMicro();
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

        void ToggleAudio() {
            AgoraVoiceController.Instance.ToggleAudio();

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