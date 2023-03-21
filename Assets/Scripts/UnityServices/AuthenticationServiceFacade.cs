using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace Project.UnityServices.Auth {
    public class AuthenticationServiceFacade {
        public async Task InitializeAndSignInAsync() {

            try {
                await Unity.Services.Core.UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn) {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception e) {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                Debug.LogError("Errror while initializing and authenticating: " + reason);
                throw;
            }
        }
    }
}

