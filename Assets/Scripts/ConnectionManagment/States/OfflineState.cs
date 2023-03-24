using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.ConnectionManagment {

    public class OfflineState : ConnectionState {

        public override void Enter() {
            m_ConnectionManager.NetworkManager.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }

        public override void Exit() {
        }

        public override void StartGame() {
            m_ConnectionManager.ChangeState(
                m_ConnectionManager.m_SelectCharState
                );

        }
    }
}