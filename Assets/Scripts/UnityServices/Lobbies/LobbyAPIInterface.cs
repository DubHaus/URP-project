using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Project.UnityServices.Lobbies {

    public class LobbyAPIInterface {
        const int k_MaxLobbiesToShow = 16; // If more are necessary, consider retrieving paginated results or using filters.
        public async Task<Lobby> CreateLobby(
            string requesterUasId,
            string lobbyName,
            int maxPlayers,
            bool isPrivate,
            Dictionary<string, PlayerDataObject> hostUserData,
            Dictionary<string, DataObject> lobbyData
        ) {

            CreateLobbyOptions createOptions = new CreateLobbyOptions() {
                IsPrivate = isPrivate,
                Player = new Player(id: requesterUasId, data: hostUserData),
                Data = lobbyData
            };

            return await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createOptions);
        }

        public async Task RemovePlayerFromLobby(string requesterUasId, string lobbyId) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, requesterUasId);
            }
            catch (LobbyServiceException e)
                when (e is { Reason: LobbyExceptionReason.PlayerNotFound }) {
                // If Player is not found, they have already left the lobby or have been kicked out. No need to throw here
            }
        }

        public async Task<Lobby> JoinLobbyByCode(string requesterUasId, string lobbyCode, Dictionary<string, PlayerDataObject> localUserData) {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions() {
                Player = new Player(id: requesterUasId, data: localUserData)
            };
            return await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
        }
        
        public async Task<Lobby> JoinLobbyById(string requesterUasId, string lobbyId, Dictionary<string, PlayerDataObject> localUserData) {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions() {
                Player = new Player(id: requesterUasId, data: localUserData)
            };
            return await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
        }

        public async Task<Lobby> UpdatePlayer(string lobbyId, string playerId, Dictionary<string, PlayerDataObject> data, string allocationId, string connectionInfo) {
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions() {
                Data = data,
                AllocationId = allocationId,
                ConnectionInfo = connectionInfo
            };
            return await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, updatePlayerOptions);
        }

        public async Task<QueryResponse> QueryAllLobbies() {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions() {
                Count = k_MaxLobbiesToShow
            };

            return await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
        }

        public async Task<Lobby> UpdateLobby(string lobbyId, Dictionary<string, DataObject> data, bool shouldLock) {
            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions() { Data = data, IsLocked = shouldLock };
            return await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateLobbyOptions);
        }

        public async Task DeleteLobby(string lobbyId) {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
        }

        public async void SendHeartbeatPing(string lobbyId) {
            await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
        }

        public async Task<ILobbyEvents> SubscribeToLobby(string lobbyId, LobbyEventCallbacks eventCallbacks) {
            return await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, eventCallbacks);
        }
    }
}