using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.GameSession;
using Project.UnityServices.Auth;
using Project.VoiceChatUtils;
using VContainer;
using VContainer.Unity;

namespace Project.Gameplay.GameState {

    public class MainMenuState : GameStateBehaviour {

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;
        [Inject] AudioChannel m_AudioChannel;
        [Inject] GameSessionManager m_GameSessionManager;

        public override GameState ActiveState => GameState.MainMenu;

        [SerializeField] MainScreenManager mainScreenManager;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.RegisterComponent(mainScreenManager);
        }

        protected override void Awake() {
            base.Awake();

            m_GameSessionManager.SetLocalPlayerId(m_AuthServiceFacade.PlayerId);
            
            uint audioUserId = (uint)m_AuthServiceFacade.PlayerId.GetHashCode(); // generate id from playerId 
            m_AudioChannel.SetLocalUser(audioUserId);
        }
    }
}