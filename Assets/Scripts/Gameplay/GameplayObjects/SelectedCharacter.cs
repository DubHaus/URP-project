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

        public bool isSelected {
            get {
                return m_CharacterSelectState.selectedCharacter == character;
            }
        }

        private void Update() {
            if (isSelected) {
                FreeCameraSystem.Instance.Focus(gameObject.transform);
            }
        }
    }
}

