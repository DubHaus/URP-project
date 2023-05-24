
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.VoiceChatUtils;
using VContainer;

namespace Project.ConnectionManagment {
    public class ClientConnectedState : ConnectionState {
        [Inject] AudioChannel m_AudioChannel;
        public override void Enter() {
            Debug.Log("Client connected to server");
            m_AudioChannel.JoinChannel();
        }

        public override void Exit() {
            m_AudioChannel.LeaveChannel();
        }

    }
}