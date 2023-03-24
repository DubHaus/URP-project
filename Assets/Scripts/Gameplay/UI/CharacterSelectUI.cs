using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Project.ConnectionManagment;
using Project.Gameplay.GameState;
using VContainer;

namespace Project.Gameplay.UI {

    public class CharacterSelectUI : MonoBehaviour {

        [SerializeField] CharacterSelectState m_CharacterSelectState;

        [SerializeField] GameObject boyPrefab;
        [SerializeField] GameObject girlPrefab;

        [SerializeField] InputField m_PlayerNameInput;
        [SerializeField] InputField m_JoinCodeInput;

        [Inject] ConnectionManager m_ConnectionManager;


        public void ChangeCharacter() {
            m_CharacterSelectState.ChangeCharacter();
        }

        public void StartHost() {
            m_ConnectionManager.StartHost(m_PlayerNameInput.text,
                ((uint)m_CharacterSelectState.selectedCharacter));
        }

        public void StartClient() {
            m_ConnectionManager.StartClient(m_PlayerNameInput.text, m_JoinCodeInput.text,
                ((uint)m_CharacterSelectState.selectedCharacter));
        }
    }
}

