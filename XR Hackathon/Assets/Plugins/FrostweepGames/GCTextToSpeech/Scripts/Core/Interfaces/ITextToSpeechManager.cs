using System;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public interface ITextToSpeechManager
    {
        event Action<GetVoicesResponse, long> GetVoicesSuccessEvent;
        event Action<PostSynthesizeResponse, long> SynthesizeSuccessEvent;

        event Action<string, long> GetVoicesFailedEvent;
        event Action<string, long> SynthesizeFailedEvent;

        string PrepareLanguage(Enumerators.LanguageCode lang);

        long GetVoices(GetVoicesRequest getVoicesRequest);
        long Synthesize(PostSynthesizeRequest synthesizeRequest);
        void CancelRequest(long requestId);
        void CancelAllRequests();
    }
}