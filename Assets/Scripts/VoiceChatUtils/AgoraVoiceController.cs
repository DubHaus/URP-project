using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora;
using Agora.Rtc;


namespace Project.VoiceChatUtils {
    public class AgoraVoiceController : MonoBehaviour {

        public static AgoraVoiceController Instance { get; private set; }

        [SerializeField] string _appID;
        [SerializeField] string _channelName;
        [SerializeField] string _token;

        public uint remoteUid;
        private ILocalSpatialAudioEngine localSpatial;
        internal IRtcEngine RtcEngine;

        void Awake() {
            DontDestroyOnLoad(gameObject);

            if (Instance != null && Instance != this) {
                Debug.LogError("More than one PlayerInputController instance");
            }
            else {
                Instance = this;
            }
        }

        void Start() {
            SetupVoiceSDKEngine();
            ConfigureSpatialAudioEngine();
            InitEventHandler();
        }

        private void SetupVoiceSDKEngine() {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            RtcEngineContext context = new RtcEngineContext(_appID, 0,
                CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);

            RtcEngine.Initialize(context);
        }

        private void InitEventHandler() {
            UserEventHandler handler = new UserEventHandler(this);
            RtcEngine.InitEventHandler(handler);
        }

        private void ConfigureSpatialAudioEngine() {
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

        public void Join() {
            RtcEngine.EnableAudio();
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            RtcEngine.JoinChannel(_token, _channelName);
        }

        public void UpdateSpatialAudioPosition(Vector3 sourceDistance) {
            float[] position = new float[] { sourceDistance.z, sourceDistance.x, sourceDistance.y };
            float[] forward = new float[] { sourceDistance.z, 0.0f, 0.0f };
            RemoteVoicePositionInfo remotePosInfo = new RemoteVoicePositionInfo(position, forward);
            localSpatial.UpdateRemotePosition((uint)remoteUid, remotePosInfo);
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
        private readonly AgoraVoiceController _audioSample;

        internal UserEventHandler(AgoraVoiceController audioSample) {
            _audioSample = audioSample;
        }

        // This callback is triggered when the local user joins the channel.
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed) {
            Debug.Log("Local user join channel " + connection.localUid);
        }

        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed) {
            Debug.Log("Remote user joined " + remoteUid);
            AgoraVoiceController.Instance.remoteUid = remoteUid;
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats) {
            Debug.Log("User left audio channel, with duration" + stats.duration);
        }

        public override void OnError(int err, string msg) {
            Debug.LogError("Error with Agora: " + msg);
        }
    }

}
