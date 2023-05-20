using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Gameplay.UI {

    public class TipsUI : MonoBehaviour {
        [SerializeField] TMP_Text tipText;
        static public TipsUI Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Debug.LogError("More than one TipsUI instance");
            } else {
                Instance = this;
            }
        }

        public void ShowTip(string text) {
            tipText.SetText(text);
        }
    }
}

