using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Unity.Netcode;
using Project.ConnectionManagment;
using Project.Gameplay;
using Project.Infrastructure;
using Project.UnityServices.Auth;
using Project.Utils;
using Project.Utils.Input;
using Project.UI;
using Project.UnityServices.Lobbies;


namespace Project.ApplicationLifecycle {
    public class ApplicationController : LifetimeScope {
        [SerializeField] NetworkManager m_NetworkManager;
        [SerializeField] ConnectionManager m_ConnectionManager;
        [SerializeField] GameSessionManager m_GameSessionManager;
        [SerializeField] PlayerInputController m_PlayerInputController;
        [SerializeField] UpdateRunner m_UpdateRunner;

        LobbyServiceFacade m_LobbyServiceFacade;
        protected override void Configure(IContainerBuilder builder) {
            builder.RegisterComponent(m_ConnectionManager);
            builder.RegisterComponent(m_GameSessionManager);
            builder.RegisterComponent(m_NetworkManager);
            builder.RegisterComponent(m_PlayerInputController);
            builder.RegisterComponent(m_UpdateRunner);

            builder.Register<LocalLobby>(Lifetime.Singleton);
            builder.Register<LocalLobbyUser>(Lifetime.Singleton);
            builder.Register<LocalUserProfile>(Lifetime.Singleton);
            builder.Register<ProfileManager>(Lifetime.Singleton);

            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);

            //LobbyServiceFacade is registered as entrypoint because it wants a callback after container is built to do it's initialization
            builder.RegisterEntryPoint<LobbyServiceFacade>(Lifetime.Singleton).AsSelf();
        }

        private void Start() {
            m_LobbyServiceFacade = Container.Resolve<LobbyServiceFacade>();

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_UpdateRunner);
            Application.targetFrameRate = 120;
            SceneManager.LoadScene("StartScreen");
        }

        protected override void OnDestroy() {
            m_LobbyServiceFacade?.EndTracking();
            base.OnDestroy();
        }

        IEnumerator LeaveBeforeQuit() {
            // We want to quit anyways, so if anything happens while trying to leave the Lobby, log the exception then carry on
            try {
                m_LobbyServiceFacade.EndTracking();
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
            yield return null;
            Application.Quit();
        }

    }
}