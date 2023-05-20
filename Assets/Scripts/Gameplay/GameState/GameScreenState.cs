using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.UnityServices.Auth;
using VContainer;
using VContainer.Unity;
using Project.UI;

namespace Project.Gameplay.GameState {

    public class GameScreenState : GameStateBehaviour {

        [Inject] GameSessionManager m_GameSessionManager;
        [SerializeField] GameScreenManager m_GameScreenManager;
        public override GameState ActiveState => GameState.GameScreen;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);
            
            builder.RegisterComponent(m_GameScreenManager);
        }
    }
}