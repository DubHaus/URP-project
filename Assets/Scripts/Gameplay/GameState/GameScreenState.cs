using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.GameSession;
using Project.VoiceChatUtils;
using VContainer;
using VContainer.Unity;

namespace Project.Gameplay.GameState {

    public class GameScreenState : GameStateBehaviour {

        [Inject] GameSessionManager m_GameSessionManager;
        [Inject] AudioChannel m_AudioChannel;
        [SerializeField] GameScreenManager m_GameScreenManager;

        bool _audioUserConnected;

        public override GameState ActiveState => GameState.GameScreen;
        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.RegisterComponent(m_GameScreenManager);
        }

    }
}