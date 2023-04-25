using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.ChessBoard {
    public class ChessBoardUI : MonoBehaviour {
        static public ChessBoardUI Instance { get; private set; }

        [SerializeField] ChessBoardNetworkController chessBoardNetworkController;
        [SerializeField] TMP_Text tipText;
        [SerializeField] Button startGameButton;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Debug.LogError("More than one ChessBoardUI instance");
            } else {
                Instance = this;
            }
        }

        public void Start() {
            startGameButton.onClick.AddListener(StartGame);
        }

        public void ShowText(string text) {
            tipText.text = text;
        }

        public void ShowStartButton() {
            startGameButton.gameObject.SetActive(true);
        }

        public void HideStartButton() {
            startGameButton.gameObject.SetActive(false);
        }

        private void StartGame() {
            chessBoardNetworkController.ActiveState.StartGame();
        }

        private void OnDestroy() {
            startGameButton.onClick.RemoveListener(StartGame);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
