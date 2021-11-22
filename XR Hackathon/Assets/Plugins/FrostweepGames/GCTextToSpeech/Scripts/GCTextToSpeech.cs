using FrostweepGames.Plugins.Core;
using System;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public class GCTextToSpeech : MonoBehaviour
    {
        public event Action<GetVoicesResponse, long> GetVoicesSuccessEvent;
        public event Action<PostSynthesizeResponse, long> SynthesizeSuccessEvent;

        public event Action<string, long> GetVoicesFailedEvent;
        public event Action<string, long> SynthesizeFailedEvent;


        private static GCTextToSpeech _Instance;
        public static GCTextToSpeech Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var obj = Resources.Load<GameObject>("Prefabs/GCTextToSpeech");

                    if (obj != null)
                    {
                        obj.name = "[Singleton]GCTextToSpeech";
                        _Instance = obj.GetComponent<GCTextToSpeech>();
                    }
                    else
                        _Instance = new GameObject("[Singleton]GCTextToSpeech").AddComponent<GCTextToSpeech>();
                }

                return _Instance;
            }
        }

        private ITextToSpeechManager _textToSpeechManager;
        private IMediaManager _mediaManager;

        public ServiceLocator ServiceLocator { get { return ServiceLocator.Instance; } }

        [Header("Prefab Object Settings")]
        public bool isDontDestroyOnLoad = false;
        public bool isFullDebugLogIfError = false;

        [Header("Prefab Fields")]
        public string apiKey = string.Empty;

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            _Instance = this;

            ServiceLocator.Register<ITextToSpeechManager>(new TextToSpeechManager());
            ServiceLocator.Register<IMediaManager>(new MediaManager());
            ServiceLocator.InitServices();

            _textToSpeechManager = ServiceLocator.Get<ITextToSpeechManager>();
            _mediaManager = ServiceLocator.Get<IMediaManager>();

            _textToSpeechManager.GetVoicesSuccessEvent += GetVoicesSuccessEventHandler;
            _textToSpeechManager.SynthesizeSuccessEvent += SynthesizeSuccessEventHandler;

            _textToSpeechManager.GetVoicesFailedEvent += GetVoicesFailedEventHandler;
            _textToSpeechManager.SynthesizeFailedEvent += SynthesizeFailedEventHandler;
        }

        private void Update()
        {
            if (_Instance == this)
            {
                ServiceLocator.Instance.Update();
            }
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _textToSpeechManager.GetVoicesSuccessEvent -= GetVoicesSuccessEventHandler;
                _textToSpeechManager.SynthesizeSuccessEvent -= SynthesizeSuccessEventHandler;

                _textToSpeechManager.GetVoicesFailedEvent -= GetVoicesFailedEventHandler;
                _textToSpeechManager.SynthesizeFailedEvent -= SynthesizeFailedEventHandler;

                _Instance = null;
                ServiceLocator.Instance.Dispose();
            }
        }

        public string PrepareLanguage(Enumerators.LanguageCode lang)
        {
            return _textToSpeechManager.PrepareLanguage(lang);
        }

        public AudioClip GetAudioClipFromBase64(string value, Enumerators.AudioEncoding audioEncoding)
        {
           return _mediaManager.GetAudioClipFromBase64String(value, audioEncoding);
        }

        public long GetVoices(GetVoicesRequest getVoicesRequest)
        {
            return _textToSpeechManager.GetVoices(getVoicesRequest);
        }

        public long Synthesize(string content, VoiceConfig voiceConfig, bool ssml = false, double pitch = 1.0, double speakingRate = 1.0, double sampleRateHertz = Constants.DEFAULT_SAMPLE_RATE)
        {
            SynthesisInput synthesisInput = null;

            if (ssml)
                synthesisInput = new SynthesisInputSSML() { ssml = content };
            else
                synthesisInput = new SynthesisInputText() { text = content };

            return _textToSpeechManager.Synthesize(new PostSynthesizeRequest()
            {
                audioConfig = new AudioConfig()
                {
                    audioEncoding = Constants.DEFAULT_AUDIO_ENCODING,
                    pitch = pitch,
                    sampleRateHertz = sampleRateHertz,
                    speakingRate = speakingRate,
                    volumeGainDb = Constants.DEFAULT_VOLUME_GAIN_DB
                },
                input = synthesisInput,
                voice = new VoiceSelectionParams()
                {
                    languageCode = voiceConfig.languageCode,
                    name = voiceConfig.name,
                    ssmlGender = voiceConfig.gender
                }
            });
        }

        public void CancelRequest(long requestId)
		{
            _textToSpeechManager.CancelRequest(requestId);
        }

        public void CancelAllRequests()
        {
            _textToSpeechManager.CancelAllRequests();
        }

        private void GetVoicesFailedEventHandler(string data, long requestId)
        {
            if (GetVoicesFailedEvent != null)
                GetVoicesFailedEvent(data, requestId);
        }

        private void SynthesizeFailedEventHandler(string data, long requestId)
        {
            if (SynthesizeFailedEvent != null)
                SynthesizeFailedEvent(data, requestId);
        }

        private void GetVoicesSuccessEventHandler(GetVoicesResponse data, long requestId)
        {
            if (GetVoicesSuccessEvent != null)
                GetVoicesSuccessEvent(data, requestId);
        }


        private void SynthesizeSuccessEventHandler(PostSynthesizeResponse data, long requestId)
        {
            if (SynthesizeSuccessEvent != null)
                SynthesizeSuccessEvent(data, requestId);
        }
    }
}