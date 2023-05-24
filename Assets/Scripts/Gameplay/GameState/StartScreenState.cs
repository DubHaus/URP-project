using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.GameSession;
using Project.UnityServices.Auth;
using VContainer;
using UnityEngine.SceneManagement;

namespace Project.Gameplay.GameState {

    public class StartScreenState : GameStateBehaviour {

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;

        public override GameState ActiveState => GameState.StartScreen;
        
        protected override void Awake() {
            base.Awake();

            TrySignIn();
        }

        private async void TrySignIn() {
            try {
                await m_AuthServiceFacade.InitializeAndSignInAsync();
                SceneManager.LoadScene("MainMenu");
            }
            catch (Exception err) {
                Debug.LogError("Sign in failed: " + err);
            }
        }
    }
}