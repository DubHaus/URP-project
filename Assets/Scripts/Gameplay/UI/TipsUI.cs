using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Gameplay.UI {

    public class TipsUI : MonoBehaviour {
        [SerializeField] TMP_Text tipText;
        static public TipsUI Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake() {
            if (Instance != null && Instance != this) {
                Debug.LogError("More than one TipsUI instance");
            } else {
                Instance = this;
            }
        }

        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void ShowTip(string text) {
            tipText.SetText(text);
        }
    }
}

