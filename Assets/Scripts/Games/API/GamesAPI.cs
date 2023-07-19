using System;
using UnityEngine;

namespace Project.Games.API {

    public enum InputType {
        Move,
        PrimaryAction,
        SecondaryAction
    };

    public struct InputInfo<T> {
        T value;
        public InputType InputType;

        public InputInfo(T value, InputType inputType) {
            this.value = value;
            InputType = inputType;
        }

        public T GetValue() {
            return value;
        }
    }

    public class Player: MonoBehaviour {

        public void MovePlayer(Vector3 to) {
            Debug.Log($"MovePlayer to {to}");
        }

        protected virtual void OnInput(InputInfo<Vector2> input) { }

        void InputHandler() {
            OnInput(new InputInfo<Vector2>(Vector2.zero, InputType.PrimaryAction)); // example
        }

    }

    public struct PlayerInfo {
        public string PlayerName;
        public string PlayerID;

        public PlayerInfo(string playerName, string playerID) {
            PlayerID = playerID;
            PlayerName = playerName;
        }
    }

    public class GameInfoAPI {

        public Action<PlayerInfo> playerJoined;

        string gameName;
        string status;
        bool canPlayerJoin;

        public GameInfoAPI() { }

        public GameInfoAPI(string gameName, string status, bool canPlayerJoin) {
            this.gameName = gameName;
            this.status = status;
            this.canPlayerJoin = canPlayerJoin;
        }

        public void UpdateGameInfo(GameInfo info) {
            Debug.Log($"UpdateGameInfo: " +
                $"GameName: {info.GameName}" +
                $"Status: {info.Status};" +
                $"CanPlayerJoin: {info.CanPlayerJoin}");
        }
    }

    public struct GameInfo {
        public string GameName;
        public string Status;
        public bool CanPlayerJoin;

        public GameInfo(string gameName, string status, bool canPlayerJoin) {
            GameName = gameName;
            Status = status;
            CanPlayerJoin = canPlayerJoin;
        }
    }
}