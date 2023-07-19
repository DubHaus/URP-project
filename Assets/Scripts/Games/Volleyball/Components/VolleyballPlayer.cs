using System;
using Project.Games.API;
using UnityEngine;

namespace Games.Volleyball {
    public class VolleyballPlayer : Player {
        public Action jump;
        public Action punch;

        string playerId;
        public string PlayerId => playerId;

        public VolleyballPlayer(string playerId) {
            this.playerId = playerId;
        }

        protected override void OnInput(InputInfo<Vector2> input) {
            switch (input.InputType) {
                case InputType.PrimaryAction:
                    Jump();
                    break;
                case InputType.SecondaryAction:
                    Punch();
                    break;
            }
        }

        void Jump() {
            jump?.Invoke();
        }

        void Punch() {
            punch?.Invoke();
        }
    }
}