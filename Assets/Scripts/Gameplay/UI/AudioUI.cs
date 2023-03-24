using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Project.VoiceChatUtils;
using System.Threading;

namespace Project.Gameplay.UI {

    public class AudioUI : MonoBehaviour {
        [SerializeField] Text muteButtonText;
        bool muted = false;


        public void Mute () {
            muted = !muted;
            AgoraVoiceController.Instance.Mute(muted);

        }

        private void Update()
        {
            muteButtonText.text = muted ? "Unmute" : "Mute";
        }

    }
}

