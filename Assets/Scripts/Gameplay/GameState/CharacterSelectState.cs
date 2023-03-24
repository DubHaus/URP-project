using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Gameplay.UI;
using Project.UnityServices.Auth;
using VContainer;
using VContainer.Unity;
using Project.Gameplay.GameplayObjects;

namespace Project.Gameplay.GameState {

    public enum Character {
        Boy = 11,
        Girl = 22,
    }

    public class CharacterSelectState : GameStateBehaviour {

        public override GameState ActiveState { get { return GameState.CharacterSelect; } }

        [SerializeField] CharacterSelectUI m_CharacterSelectUI;

        public Character selectedCharacter { get; private set; } = Character.Boy;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.RegisterComponent(m_CharacterSelectUI);
        }

        public void ChangeCharacter() {
            this.selectedCharacter = selectedCharacter == Character.Boy ? Character.Girl : Character.Boy;
        }

    }
}
