using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Gameplay.UI {

    public class TipsUI : MonoBehaviour {
        [SerializeField] TMP_Text tipText;
        // Start is called before the first frame update
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

