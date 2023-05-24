using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.VoiceChatUtils {

    public enum AudioDeviceStatus {
        Active,
        Disabled,
        Disconnected
    }

    public class AudioChannel {
        public Action<AudioUser> localUserChanged;
        public Action<uint> userJoined;
        public Action<AudioUser> userChanged;

        Dictionary<uint, AudioUser> users = new Dictionary<uint, AudioUser>();
        AudioUser localUser;
        AgoraVoiceControllerAPIInterface m_AgoraVoiceControllerAPIInterface;
        bool connectedToChannel;

        public struct AudioUser {
            public uint ID;
            public string PlayerID;
            public AudioDeviceStatus MicroStatus;
            public AudioDeviceStatus AudioStatus;

            public AudioUser(uint id, string playerID, AudioDeviceStatus microStatus, AudioDeviceStatus audioStatus) {
                ID = id;
                PlayerID = playerID;
                MicroStatus = microStatus;
                AudioStatus = audioStatus;
            }
        }

        public AudioChannel() {
            m_AgoraVoiceControllerAPIInterface = new AgoraVoiceControllerAPIInterface();
            m_AgoraVoiceControllerAPIInterface.Init();
            m_AgoraVoiceControllerAPIInterface.OnUserJoined += OnUserJoined;
            m_AgoraVoiceControllerAPIInterface.OnRemoteUserMutedMicro += OnUserMutedMicro;
            m_AgoraVoiceControllerAPIInterface.OnRemoteUserMutedAudio += OnUserMutedAudio;
        }

        public List<AudioUser> Users => users.Values.ToList();
        public AudioUser LocalUser => localUser;

        public AudioDeviceStatus LocalAudioStatus => localUser.AudioStatus;
        public AudioDeviceStatus LocalMicroStatus => localUser.MicroStatus;

        public void SetLocalUser(uint audioUserId) {
            localUser = new AudioUser(audioUserId, null, AudioDeviceStatus.Active, AudioDeviceStatus.Active);
        }

        public void SetMuteLocalMicro(bool mute) {
            if (connectedToChannel) {
                if (m_AgoraVoiceControllerAPIInterface.SetMuteLocalMicro(mute)) {
                    localUser.MicroStatus = mute ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
                    OnLocalUserChanged();
                }
                else {
                    Debug.LogError("Error in agora while muting local user micro");
                }
            }
            else {
                localUser.MicroStatus = mute ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
            }
        }

        public void SetMuteLocalAudio(bool mute) {
            if (connectedToChannel) {
                if (m_AgoraVoiceControllerAPIInterface.SetMuteLocalAudio(mute)) {
                    localUser.AudioStatus = mute ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
                    OnLocalUserChanged();
                }
                else {
                    Debug.LogError("Error in agora while muting local user audio");
                }
            }
            else {
                localUser.AudioStatus = mute ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
            }
        }

        public void ToggleMicro() {
            SetMuteLocalMicro(localUser.MicroStatus == AudioDeviceStatus.Active);
        }

        public void ToggleAudio() {
            SetMuteLocalAudio(localUser.AudioStatus == AudioDeviceStatus.Active);
        }

        void OnUserMutedMicro(uint id, bool muted) {
            if (users.TryGetValue(id, out var user)) {
                user.MicroStatus = muted ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
                users[id] = user;
                OnUserChanged(user);
            }
        }

        void OnUserMutedAudio(uint id, bool muted) {
            if (users.TryGetValue(id, out var user)) {
                user.AudioStatus = muted ? AudioDeviceStatus.Disabled : AudioDeviceStatus.Active;
                users[id] = user;
                OnUserChanged(user);
            }
        }

        public AudioUser? GetUserByPlayerId(string id) {
            return Users.Find(user => user.PlayerID == id);
        }

        public void JoinChannel() {
            Debug.Log("JoinChannel " + localUser.ID);
            m_AgoraVoiceControllerAPIInterface.Join(
                localUser.ID,
                localUser.AudioStatus != AudioDeviceStatus.Active,
                localUser.MicroStatus != AudioDeviceStatus.Active
            );
            connectedToChannel = true;
        }

        void OnUserJoined(uint id) {
            userJoined?.Invoke(id);
        }

        public void AddUser(uint id, string playerId) {
            users.Add(id, new AudioUser(id, playerId, AudioDeviceStatus.Disabled, AudioDeviceStatus.Disabled));
        }

        public void LeaveChannel() {
            m_AgoraVoiceControllerAPIInterface.Leave();
        }

        void OnLocalUserChanged() {
            localUserChanged?.Invoke(localUser);
        }

        void OnUserChanged(AudioUser user) {
            userChanged?.Invoke(user);
        }
    }
}