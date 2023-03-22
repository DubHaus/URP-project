
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.VoiceChatUtils;

namespace Project.ConnectionManagment {
    public class ClientConnectedState : ConnectionState {
        public override void Enter() {
            Debug.Log("Client connected to server");
            AgoraVoiceController.Instance.Join();
        }

        public override void Exit() {
            AgoraVoiceController.Instance.Leave();
        }

    }
}