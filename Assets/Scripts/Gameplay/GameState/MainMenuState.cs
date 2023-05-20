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

    public class MainMenuState : GameStateBehaviour {

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;

        public override GameState ActiveState => GameState.MainMenu;

        [SerializeField] MainScreenManager mainScreenManager;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);
            
            builder.RegisterComponent(mainScreenManager);
        }
    }
}