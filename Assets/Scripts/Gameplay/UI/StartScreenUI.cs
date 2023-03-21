using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.ConnectionManagment;
using VContainer;

namespace Project.Gameplay.UI {

    public class StartScreenUI : MonoBehaviour {

        [SerializeField] InputField m_PlayerNameInput;
        [SerializeField] InputField m_JoinCodeInput;

        [Inject] ConnectionManager m_ConnectionManager;

        public void StartHost() {
            m_ConnectionManager.StartHost(m_PlayerNameInput.text);
        }

        public void StartClient() {
            m_ConnectionManager.StartClient(m_PlayerNameInput.text, m_JoinCodeInput.text);
        }
    }
}

