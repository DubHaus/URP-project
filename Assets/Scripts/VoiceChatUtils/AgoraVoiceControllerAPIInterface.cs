using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora;
using Agora.Rtc;


namespace Project.VoiceChatUtils {
    public class AgoraVoiceControllerAPIInterface {
        public Action<uint> OnUserJoined;
        public Action<uint, bool> OnRemoteUserMutedAudio;
        public Action<uint, bool> OnRemoteUserMutedMicro;

        string _appID = "9e59f51bf0ba4b208502f338d3ef4dc3";
        string _channelName = "unity";
        string _token = "007eJxTYLidPefrUpv0U94R9iVaTasabnffFbcq7l/WtmtL7JPLMtEKDJapppZppoZJaQZJiSZJRgYWpgZGacbGFinGqWkmKcnGLhdzUxoCGRlme9azMDJAIIjPylCal1lSycAAANgzITI=";
        ILocalSpatialAudioEngine localSpatial;
        internal IRtcEngine RtcEngine;

        public void Init() {
            SetupVoiceSDKEngine();
            ConfigureSpatialAudioEngine();
            InitEventHandler();
        }

        void SetupVoiceSDKEngine() {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            RtcEngineContext context = new RtcEngineContext(_appID, 0,
                CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);

            RtcEngine.Initialize(context);
        }

        void InitEventHandler() {
            UserEventHandler handler = new UserEventHandler(this);
            RtcEngine.InitEventHandler(handler);
        }

        void ConfigureSpatialAudioEngine() {
            RtcEngine.EnableAudio();
            RtcEngine.EnableSpatialAudio(true);
            LocalSpatialAudioConfig localSpatialAudioConfig = new LocalSpatialAudioConfig();
            localSpatialAudioConfig.rtcEngine = RtcEngine;
            localSpatial = RtcEngine.GetLocalSpatialAudioEngine();
            localSpatial.Initialize();
            //By default Agora subscribes to the audio streams of all remote users.
            //Unsubscribe all remote users; otherwise, the audio reception range you set
            //is invalid.
            //localSpatial.MuteLocalAudioStream(true);
            //localSpatial.MuteAllRemoteAudioStreams(true);

            localSpatial.SetAudioRecvRange(20);
            localSpatial.SetDistanceUnit(1);

            float[] position = new float[] { 0.0f, 0.0f, 0.0f };
            float[] axisForward = new float[] { 1.0F, 0.0F, 0.0F };
            float[] axisRight = new float[] { 0.0F, 1.0F, 0.0F };
            float[] axisUp = new float[] { 0.0F, 0.0F, 1.0F };

            localSpatial.UpdateSelfPosition(position, axisForward, axisRight, axisUp);
        }

        public void Join(uint userId, bool defaultAudioMuted, bool defaultMicroMuted) {
            RtcEngine.EnableAudio();
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            RtcEngine.JoinChannel(_token, _channelName, "", userId);

            // set default values of micro and audio before joining
            localSpatial.MuteAllRemoteAudioStreams(defaultAudioMuted);
            localSpatial.MuteLocalAudioStream(defaultMicroMuted);

        }

        public void UpdateSpatialAudioPosition(uint playerId, Vector3 sourceDistance) {
            float[] position = { sourceDistance.z, sourceDistance.x, sourceDistance.y };
            float[] forward = { sourceDistance.z, 0.0f, 0.0f };
            RemoteVoicePositionInfo remotePosInfo = new RemoteVoicePositionInfo(position, forward);
            localSpatial.UpdateRemotePosition(playerId, remotePosInfo);
        }

        public bool SetMuteLocalMicro(bool mute) {
            int success = 0;
            return localSpatial.MuteLocalAudioStream(mute) == success;
        }

        public bool SetMuteLocalAudio(bool mute) {
            int success = 0;
            return localSpatial.MuteAllRemoteAudioStreams(mute) == success;
        }

        public void Leave() {
            RtcEngine.LeaveChannel();
            RtcEngine.DisableAudio();
        }

        private void OnDestroy() {
            Leave();
            RtcEngine.Dispose();
            RtcEngine = null;
        }


    }
    internal class UserEventHandler : IRtcEngineEventHandler {
        readonly AgoraVoiceControllerAPIInterface m_AgoraVoiceControllerAPIInterface;

        internal UserEventHandler(AgoraVoiceControllerAPIInterface agoraVoiceControllerAPIInterface) {
            m_AgoraVoiceControllerAPIInterface = agoraVoiceControllerAPIInterface;
        }

        // This callback is triggered when the local user joins the channel.
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed) {
            Debug.Log("Local user join channel " + connection.channelId);
        }

        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed) {
            Debug.Log("Remote user joined " + remoteUid);
            m_AgoraVoiceControllerAPIInterface.OnUserJoined?.Invoke(remoteUid);
        }

        public override void OnUserMuteAudio(RtcConnection connection, uint remoteUid, bool muted) {
            m_AgoraVoiceControllerAPIInterface.OnRemoteUserMutedMicro?.Invoke(remoteUid, muted);
        }

        public override void OnRemoteAudioStateChanged(RtcConnection connection, uint remoteUid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed) {
            Debug.Log("OnRemoteAudioStateChanged " +  state);
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats) {
            Debug.Log("User left audio channel, with duration" + stats.duration);
        }

        public override void OnError(int err, string msg) {
            Debug.LogError("Error with Agora: " + msg);
        }
    }

}