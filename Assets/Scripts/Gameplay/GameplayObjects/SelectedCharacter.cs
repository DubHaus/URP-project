using System.Collections;
using System.Collections.Generic;
using Project.Gameplay.GameState;
using UnityEngine;
using VContainer;
using Project.CameraUtils;

namespace Project.Gameplay.GameplayObjects {

    public class SelectedCharacter : MonoBehaviour {

        [SerializeField] CharacterSelectState m_CharacterSelectState;
        [SerializeField] Project.Gameplay.GameState.Character character;
        [SerializeField] FreeCameraSystem cameraSystem;

        public bool isSelected {
            get {
                return m_CharacterSelectState.selectedCharacter == character;
            }
        }

        private void Update() {
            if (isSelected) {
                cameraSystem.Focus(gameObject.transform);
            }
        }
    }
}

