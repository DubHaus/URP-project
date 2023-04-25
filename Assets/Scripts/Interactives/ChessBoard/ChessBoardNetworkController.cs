using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project.Gameplay.UI;
using Unity.Collections;
using Project.Interactive;

namespace Project.ChessBoard {



    public enum PlayerState {
        NotReady,
        Ready
    }

    public enum ChessPicesColor {
        White,
        Black
    }

    [Serializable]
    public enum ActiveGameState {
        Idle,
        WaitingForPlayers,
        ChoosingCharacter,
        PlayingGame
    }


    public struct Player : INetworkSerializable, System.IEquatable<Player> {
        public ulong id;
        public ChessPiceType character;
        public PlayerState state;

        public Player(ulong id, ChessPiceType character, PlayerState state) {
            this.id = id;
            this.character = character;
            this.state = state;

        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out id);
                reader.ReadValueSafe(out character);
                reader.ReadValueSafe(out state);
            } else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(id);
                writer.WriteValueSafe(character);
                writer.WriteValueSafe(state);
            }
        }

        public bool Equals(Player other) {
            return id == other.id && character == other.character && state == other.state;
        }
    }

    public struct Piece : INetworkSerializable, System.IEquatable<Piece> {
        public ChessPiceType type;
        public Vector2 position;
        public ChessPicesColor color;
        public int id;

        public Piece(ChessPiceType type, ChessPicesColor color, Vector2 position) {
            this.type = type;
            this.color = color;
            this.position = position;
            id = (int)type + (int)color;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out type);
                reader.ReadValueSafe(out position);
                reader.ReadValueSafe(out color);
                reader.ReadValueSafe(out id);
            } else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(type);
                writer.WriteValueSafe(position);
                writer.WriteValueSafe(color);
                writer.WriteValueSafe(id);
            }
        }

        public bool Equals(Piece other) {
            return type == other.type && position == other.position && color == other.color && id == other.id;
        }
    }


    public class ChessBoardNetworkController : Interactive.Interactive {

        Dictionary<ChessPiceType, int> CHESS_PIECES_START_POSITION = new Dictionary<ChessPiceType, int>() {
            { ChessPiceType.Bishop, 0 },
            { ChessPiceType.Knight, 1 },
            { ChessPiceType.Rook, 2},
            { ChessPiceType.King, 3 },
            { ChessPiceType.Queen, 4},
            {ChessPiceType.Rook2, 5},
            { ChessPiceType.Knight2, 6},
            { ChessPiceType.Bishop2, 7},
        };



        static public ChessBoardNetworkController Instance;
        static public int[,] ChessBoardSquaresMatrix = new int[8, 8] {
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
            { 0, 1, 2, 3, 4, 5, 6, 7 },
        };

        [SerializeField] public ChessBoard ChessBoard;
        public NetworkVariable<ActiveGameState> activeState { get; } = new NetworkVariable<ActiveGameState>(ActiveGameState.Idle);
        public NetworkList<Player> players;
        public NetworkList<Piece> pieces;

        public ChessBoardState ActiveState = new IdleState();

        private List<(Piece, ChessPiece)> piecesMap = new List<(Piece, ChessPiece)>();

        private void Awake() {
            if (Instance != null) {
                Debug.LogError("More than one instance of ChessBoardNetworkController");
            } else {
                Instance = this;
            }

            List<Piece> localPieces = new List<Piece>();
            foreach (ChessPicesColor color in Enum.GetValues(typeof(ChessPicesColor))) {
                foreach (ChessPiceType type in Enum.GetValues(typeof(ChessPiceType))) {
                    localPieces.Add(new Piece(type, color, new Vector2(CHESS_PIECES_START_POSITION[type], color == ChessPicesColor.White ? 0 : 7)));
                }
            }
            pieces = new NetworkList<Piece>(localPieces);

            players = new NetworkList<Player>();
        }

        private void Start() {
            ActiveState.ChangeState = (state) => {
                ChangeActiveStateServerRpc(state);
            };
            ActiveState.JoinGame = JoinGame;
            ActiveState.Enter();
        }


        public override void OnNetworkSpawn() {
            activeState.OnValueChanged += (_, _) => {
                // exit from pref state
                ActiveState.Exit();

                switch (activeState.Value) {
                    case ActiveGameState.Idle:
                        ActiveState = new IdleState();
                        break;
                    case ActiveGameState.WaitingForPlayers:
                        ActiveState = new WaitingForPlayersState();
                        break;
                    case ActiveGameState.ChoosingCharacter:
                        ActiveState = new ChoosingCharacterState();
                        break;
                    case ActiveGameState.PlayingGame:
                        ActiveState = new PlayingGameState();
                        break;
                }

                ActiveState.ChangeState = (state) => {
                    ChangeActiveStateServerRpc(state);
                };
                ActiveState.JoinGame = JoinGame;
                ActiveState.LeaveGame = LeaveGame;
                ActiveState.Enter();
            };

            ChessBoard.InstantiatePieces(pieces);
            pieces.OnListChanged += (changed) => {
                ChessBoard.UpdatePieces(pieces);
            };



            players.OnListChanged += (changed) => {
                Debug.Log("OnListChanged " + players.Count);
                if (IsHost) {
                    // TODO add check whether user leave or joined
                    if (changed.Value.state == PlayerState.Ready && changed.PreviousValue.state != changed.Value.state) {
                        int playersReadyCount = 0;
                        foreach (Player player in players) {
                            if (player.state == PlayerState.Ready) {
                                playersReadyCount++;
                            }
                        }
                        Debug.Log(playersReadyCount + ";" + players.Count);
                        Debug.Log(players.Count == playersReadyCount);
                        ActiveState.OnPlayerReady(players.Count == playersReadyCount);
                    } else {
                        ActiveState.OnJoinGame(players.Count);
                    }

                }
            };

        }

        public override void Interact() {
            ActiveState.Interact();
            ChessBoard.Interact();
        }

        public void UpdatePiecePosition(int id, Vector2 position) {
            foreach (var piece in pieces) {
                if (piece.id == id) {
                    pieces[pieces.IndexOf(piece)] = new Piece(piece.type, piece.color, position);
                    break;
                }
            }
        }



        private void JoinGame() {
            AddPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }


        private void LeaveGame() {
            RemovePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeActiveStateServerRpc(ActiveGameState state) {
            activeState.Value = state;
        }

        public void UpdatePlayerCharacter(ChessPiceType chessPice) {
            UpdatePlayerCharacterServerRpc(NetworkManager.LocalClientId, chessPice);
        }

        public void UpdatePlayerState(PlayerState state) {
            UpdatePlayerStateServerRpc(NetworkManager.LocalClientId, state);
        }


        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerServerRpc(ulong playerId) {
            Player player = new Player();
            player.id = playerId;
            player.state = PlayerState.NotReady;
            players.Add(player);
        }


        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerCharacterServerRpc(ulong playerId, ChessPiceType chessPice) {
            for (int idx = 0; idx < players.Count; idx++) {
                if (players[idx].id == playerId) {
                    players[idx] = new Player(
                        id: players[idx].id,
                        character: chessPice,
                        state: players[idx].state
                        );
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerStateServerRpc(ulong playerId, PlayerState state) {
            for (int idx = 0; idx < players.Count; idx++) {
                if (players[idx].id == playerId) {
                    players[idx] = new Player(
                        id: players[idx].id,
                        character: players[idx].character,
                        state: state
                        );
                    break;
                }
            }
        }



        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerServerRpc(ulong playerId) {
            //players.Remove(playerId);
        }
    }

}
