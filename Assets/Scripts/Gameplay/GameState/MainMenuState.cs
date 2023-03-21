using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.UnityServices.Auth;
using VContainer;
using VContainer.Unity;

namespace Project.Gameplay.GameState {

    public class MainMenuState : GameStateBehaviour {

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;

        public override GameState ActiveState { get { return GameState.MainMenu; } }

        [SerializeField] StartScreenUI m_StartScreenUI;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.RegisterComponent(m_StartScreenUI);
        }

        protected override void Awake() {
            base.Awake();

            TrySignIn();
        }

        private async void TrySignIn() {
            try {
                await m_AuthServiceFacade.InitializeAndSignInAsync();
            }
            catch (Exception err) {
                Debug.LogError("Sign in failed: " + err);
            }
        }
    }
}
