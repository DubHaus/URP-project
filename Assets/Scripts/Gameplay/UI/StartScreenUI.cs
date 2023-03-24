using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.ConnectionManagment;
using VContainer;

namespace Project.Gameplay.UI {

    public class StartScreenUI : MonoBehaviour {
        [Inject] ConnectionManager m_ConnectionManager;

        public void StartGame() {
            m_ConnectionManager.StartGame();
        }
    }
}

