using UnityEngine;
using System;
using Newtonsoft.Json;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.Networking;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public class TextToSpeechManager : IService, IDisposable, ITextToSpeechManager
    {
        public event Action<GetVoicesResponse, long> GetVoicesSuccessEvent;
        public event Action<PostSynthesizeResponse, long> SynthesizeSuccessEvent;

        public event Action<string, long> GetVoicesFailedEvent;
        public event Action<string, long> SynthesizeFailedEvent;

        private NetworkingService _networking;
        private GCTextToSpeech _gcTextToSpeech;

        public void Init()
        {
            _gcTextToSpeech = GCTextToSpeech.Instance;

            _networking = new NetworkingService();
            _networking.NetworkResponseEvent += NetworkResponseEventHandler;
        }

        public void Update()
        {
            _networking.Update();
        }

        public void Dispose()
        {
            _networking.NetworkResponseEvent -= NetworkResponseEventHandler;
            _networking.Dispose();
        }

        public string PrepareLanguage(Enumerators.LanguageCode lang)
        {
            return lang.ToString().Replace("_", "-");
        }

        public long GetVoices(GetVoicesRequest getVoicesRequest)
        {
            string uri = Constants.GET_LIST_VOICES;

            uri += Constants.API_KEY_PARAM + _gcTextToSpeech.apiKey;

            uri += "&languageCode=" + getVoicesRequest.languageCode;

            return _networking.SendRequest(uri, string.Empty, NetworkEnumerators.RequestType.GET,
                new object[] { Enumerators.GoogleCloudRequestType.GET_VOICES });
        }

        public long Synthesize(PostSynthesizeRequest synthesizeRequest)
        {
            string uri = Constants.POST_TEXT_SYNTHESIZE;

            uri += Constants.API_KEY_PARAM + _gcTextToSpeech.apiKey;

            return _networking.SendRequest(uri, JsonConvert.SerializeObject(synthesizeRequest), NetworkEnumerators.RequestType.POST, 
                new object[] { Enumerators.GoogleCloudRequestType.SYNTHESIZE });
        }

        public void CancelRequest(long requestId)
		{
            _networking.CancelRequest(requestId);
        }

        public void CancelAllRequests()
        {
            _networking.CancelAllRequests();
        }

        private void NetworkResponseEventHandler(NetworkResponse Response)
        {
            Enumerators.GoogleCloudRequestType googleCloudRequestType = (Enumerators.GoogleCloudRequestType)Response.Parameters[0];

            if (!string.IsNullOrEmpty(Response.Error))
            {
                ThrowFailedEvent(Response.Error + "; " + Response.Response, googleCloudRequestType, Response.RequestId);
            }
            else
            {
                if (Response.Response.Contains("Error"))
                {
                    if (_gcTextToSpeech.isFullDebugLogIfError)
                        Debug.Log(Response.Error + "\n" + Response.Response);

                    ThrowFailedEvent(Response.Response, googleCloudRequestType, Response.RequestId);
                }
                else
                    ThrowSuccessEvent(Response.Response, googleCloudRequestType, Response.RequestId);
            }
        }

        private void ThrowFailedEvent(string Error, Enumerators.GoogleCloudRequestType type, long requestId)
        {
            switch (type)
            {
                case Enumerators.GoogleCloudRequestType.GET_VOICES:
                    {
                        if (GetVoicesFailedEvent != null)
                            GetVoicesFailedEvent(Error, requestId);
                    }
                    break;
                case Enumerators.GoogleCloudRequestType.SYNTHESIZE:
                    {
                        if (SynthesizeFailedEvent != null)
                            SynthesizeFailedEvent(Error, requestId);
                    }
                    break;
                default: break;
            }
        }

        private void ThrowSuccessEvent(string data, Enumerators.GoogleCloudRequestType type, long requestId)
        {
            switch (type)
            {
                case Enumerators.GoogleCloudRequestType.GET_VOICES:
                    {
                        if (GetVoicesSuccessEvent != null)
                            GetVoicesSuccessEvent(JsonConvert.DeserializeObject<GetVoicesResponse>(data), requestId);
                    }
                    break;        
                case Enumerators.GoogleCloudRequestType.SYNTHESIZE:
                    {
                        if (SynthesizeSuccessEvent != null)
                            SynthesizeSuccessEvent(JsonConvert.DeserializeObject<PostSynthesizeResponse>(data), requestId);
                    }
                    break;
                default: break;
            }
        }
    }
}