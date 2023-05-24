using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Project.ChessBoard {

    public enum PieceOverlap {
        Friendly,
        Enemy,
        NoOverlap
    }

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
        PlayingGame,
        EndGame
    }

    public struct GameState : INetworkSerializable, System.IEquatable<GameState> {

        public ActiveGameState activeGameState;
        public ulong activePlayerId;
        public ulong loserPlayerId;
        public GameState(ActiveGameState activeGameState, ulong activePlayerId, ulong loserPlayerId) {
            this.activeGameState = activeGameState;
            this.activePlayerId = activePlayerId;
            this.loserPlayerId = loserPlayerId;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out activeGameState);
                reader.ReadValueSafe(out activePlayerId);
            } else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(activeGameState);
                writer.WriteValueSafe(activePlayerId);
            }
        }

        public bool Equals(GameState other) {
            return activeGameState == other.activeGameState && activePlayerId == other.activePlayerId && loserPlayerId == other.loserPlayerId;
        }
    }


    public struct Player : INetworkSerializable, System.IEquatable<Player> {
        public ulong id;
        public ChessPiceType character;
        public PlayerState state;
        public ChessPicesColor side;

        public Player(ulong id, ChessPiceType character, ChessPicesColor side, PlayerState state) {
            this.id = id;
            this.character = character;
            this.state = state;
            this.side = side;

        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            if (serializer.IsReader) {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out id);
                reader.ReadValueSafe(out character);
                reader.ReadValueSafe(out side);
                reader.ReadValueSafe(out state);
            } else {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(id);
                writer.WriteValueSafe(character);
                writer.WriteValueSafe(side);
                writer.WriteValueSafe(state);
            }
        }

        public bool Equals(Player other) {
            return id == other.id && character == other.character && side == other.side && state == other.state;
        }
    }

    public struct Piece : INetworkSerializable, System.IEquatable<Piece> {
        public ChessPiceType type;
        public Vector2 position;
        public ChessPicesColor color;
        public int id;

        public Piece(ChessPiceType type, ChessPicesColor color, Vector2 position, int id) {
            this.type = type;
            this.color = color;
            this.position = position;
            this.id = id;
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



        static public ChessBoardNetworkController LocalInstance;

        [SerializeField] public ChessBoard ChessBoard;
        public NetworkVariable<GameState> gameState { get; } = new NetworkVariable<GameState>(new GameState(ActiveGameState.Idle, 0, 0));
        public NetworkList<Player> players;
        public NetworkList<Piece> pieces;

        public ChessBoardState ActiveState = new IdleState();

        public Player? localPlayer {
            get {
                foreach (var player in players) {
                    if (player.id == NetworkManager.Singleton.LocalClientId) {
                        return player;
                    }
                };
                return null;
            }
        }

        public bool isLocalPlayerActive {
            get {
                return gameState.Value.activePlayerId == NetworkManager.Singleton.LocalClientId;
            }
        }

        private void Awake() {
            pieces = new NetworkList<Piece>();
            players = new NetworkList<Player>();
        }

        private void Start() {
            ActiveState.JoinGame = JoinGame;
            ActiveState.Enter();
        }


        public override void OnNetworkSpawn() {
            if (LocalInstance != null) {
                Debug.LogError("More than one local instance of ChessBoardNetworkController");
            } else {
                LocalInstance = this;
            }

            if (IsHost) {
                foreach (ChessPicesColor color in Enum.GetValues(typeof(ChessPicesColor))) {
                    foreach (ChessPiceType type in Enum.GetValues(typeof(ChessPiceType))) {
                        int randomId = UnityEngine.Random.Range(0, 10000);
                        pieces.Add(new Piece(type, color, new Vector2(CHESS_PIECES_START_POSITION[type], color == ChessPicesColor.White ? 0 : 7), randomId));
                    }
                }
            }

            gameState.OnValueChanged += (pref, _) => {
                // exit from pref state
                ActiveState.Exit();

                if (pref.activeGameState != gameState.Value.activeGameState) {
                    switch (gameState.Value.activeGameState) {
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
                        case ActiveGameState.EndGame:
                            ActiveState = new EndGameState();
                            break;
                    }
                    ActiveState.JoinGame = JoinGame;
                    ActiveState.LeaveGame = LeaveGame;
                    ActiveState.Enter();
                }

                if (pref.activePlayerId != gameState.Value.activePlayerId) {
                    Debug.Log("activePlayerId " + gameState.Value.activePlayerId + "; localClientId " + NetworkManager.LocalClientId);
                    ActiveState.OnPlayersTurn();
                }

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

                if (changed.PreviousValue.character != changed.Value.character) {
                    Debug.Log("ChooseCharacter " + changed.Value.character + "id: " + changed.Value.id + "; localId: " + NetworkManager.LocalClient.ClientId);
                    if (changed.Value.id == NetworkManager.LocalClient.ClientId) {
                        ChessBoard.ChooseCharacter(changed.Value.character, changed.Value.side);
                    }
                }
            };

        }

        public override void Interact() {
            ActiveState.Interact();
            ChessBoard.Interact();
        }

        public void UpdatePiecePosition(int id, Vector2 position) {
            UpdatePiecePositionServerRpc(id, position);
        }

        public (PieceOverlap, Piece? piece) CheckPieceOverlap(ChessPicesColor color, Vector2 position) {

            foreach (var piece in pieces) {
                if (piece.position == position) {
                    if (piece.color == color) {
                        return (PieceOverlap.Friendly, piece);
                    } else {
                        return (PieceOverlap.Enemy, piece);
                    }
                }
            }

            return (PieceOverlap.NoOverlap, null);
        }

        private void JoinGame() {
            float blackPlayers = 0;
            foreach (var player in players) {
                if (player.side == ChessPicesColor.Black) {
                    blackPlayers++;
                }
            }
            ChessPicesColor side = blackPlayers >= ((float)players.Count / 2) ? ChessPicesColor.White : ChessPicesColor.Black;
            AddPlayerServerRpc(NetworkManager.Singleton.LocalClientId, side);
        }


        private void LeaveGame() {
            //RemovePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }


        public void ChangeState(ActiveGameState state) {
            ChangeActiveStateServerRpc(state);
        }

        public void SetLoserPlayerId(ulong playerId) {
            ChangeLoserPlayerIdServerRpc(playerId);
        }


        public void UpdatePlayerCharacter(ChessPiceType chessPice) {
            UpdatePlayerCharacterServerRpc(NetworkManager.LocalClientId, chessPice);
        }

        public void UpdatePlayerState(PlayerState state) {
            UpdatePlayerStateServerRpc(NetworkManager.LocalClientId, state);
        }

        public void RemovePiece(int pieceId) {
            Debug.Log("RemovePiece " + pieceId);
            RemovePieceServerRpc(pieceId);
        }


        public void ChangeActivePlayer() {
            ulong playerId = 0;
            if (gameState.Value.activePlayerId == 0) {
                playerId = players[0].id;
            }

            for (int idx = 0; idx < players.Count; idx++) {
                if (players[idx].id == gameState.Value.activePlayerId) {
                    if (idx == players.Count - 1) {
                        playerId = players[0].id;
                    } else {
                        playerId = players[idx + 1].id;
                    }
                }
            }
            ChangeActivePlayerServerRpc(playerId);

        }


        [ServerRpc(RequireOwnership = false)]
        public void ChangeActiveStateServerRpc(ActiveGameState state) {
            gameState.Value = new GameState(state, gameState.Value.activePlayerId, gameState.Value.loserPlayerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeLoserPlayerIdServerRpc(ulong playerId) {
            gameState.Value = new GameState(gameState.Value.activeGameState, gameState.Value.activePlayerId, playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeActivePlayerServerRpc(ulong activePlayerId) {
            gameState.Value = new GameState(gameState.Value.activeGameState, activePlayerId, gameState.Value.loserPlayerId);
        }


        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerServerRpc(ulong playerId, ChessPicesColor side) {
            Player player = new Player();
            player.id = playerId;
            player.state = PlayerState.NotReady;
            player.side = side;
            players.Add(player);
        }


        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerCharacterServerRpc(ulong playerId, ChessPiceType chessPice) {
            for (int idx = 0; idx < players.Count; idx++) {
                if (players[idx].id == playerId) {
                    players[idx] = new Player(
                        id: players[idx].id,
                        character: chessPice,
                        side: players[idx].side,
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
                        state: state,
                        side: players[idx].side
                        );
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdatePiecePositionServerRpc(int id, Vector2 position) {
            foreach (var piece in pieces) {
                if (piece.id == id) {
                    pieces[pieces.IndexOf(piece)] = new Piece(piece.type, piece.color, position, piece.id);
                    break;
                }
            }
        }



        [ServerRpc(RequireOwnership = false)]
        public void RemovePieceServerRpc(int pieceId) {
            for (int idx = pieces.Count - 1; idx >= 0; idx--) {
                if (pieces[idx].id == pieceId) {
                    pieces.RemoveAt(idx);
                }
            }
        }
    }

}
