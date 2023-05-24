using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Project.GameSession;
using Project.UnityServices.Lobbies;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using VContainer;

namespace Project.UnityServices.Auth {
    public class AuthenticationServiceFacade {

        [Inject] LocalUserProfile m_LocalUserProfile;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] GameSessionManager m_GameSessionManager;


        public string PlayerId => AuthenticationService.Instance.PlayerInfo.Id;

        public async Task InitializeAndSignInAsync() {

            try {
                await Unity.Services.Core.UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn) {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    m_LocalUser.DisplayName = AuthenticationService.Instance.PlayerName;
                    m_LocalUserProfile.UpdateUserProfile(
                        new LocalUserProfile.UserProfile(
                            AuthenticationService.Instance.PlayerName,
                            AuthenticationService.Instance.IsAuthorized,
                            AuthenticationService.Instance.PlayerInfo.Id,
                            AuthenticationService.Instance.PlayerInfo.CreatedAt
                        )
                    );
                }
            }
            catch (Exception e) {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                Debug.LogError("Error while initializing and authenticating: " + reason);
                Debug.LogError(e);
                throw;
            }
        }
        public async Task<bool> EnsurePlayerIsAuthorized() {
            if (AuthenticationService.Instance.IsAuthorized) {
                return true;
            }

            try {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (AuthenticationException e) {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                Debug.Log("Authentication Error " + reason);

                //not rethrowing for authentication exceptions - any failure to authenticate is considered "handled failure"
                return false;
            }
            catch (Exception e) {
                //all other exceptions should still bubble up as unhandled ones
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                Debug.Log("Authentication Error " + reason);
                throw;
            }
        }
    }

}