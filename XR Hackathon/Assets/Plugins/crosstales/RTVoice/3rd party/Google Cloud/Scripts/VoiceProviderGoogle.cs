using UnityEngine;
using System.Collections;
using System.Linq;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

namespace Crosstales.RTVoice.Google
{
   /// <summary>
   /// Google Cloud voice provider.
   /// NOTE: This provider needs "Google Cloud Text To Speech" https://assetstore.unity.com/packages/slug/115170?aid=1011lNGT
   /// </summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_google_1_1_voice_provider_google.html")]
   public class VoiceProviderGoogle : Provider.BaseCustomVoiceProvider
   {
      private bool isLoading;

      #region Properties

      public override string AudioFileExtension => ".wav";

      public override AudioType AudioFileType => AudioType.WAV;

      public override string DefaultVoiceName => "en-US-Standard-B";

      public override bool isWorkingInEditor => false;

      public override bool isWorkingInPlaymode => true;

      public override bool isPlatformSupported => true;

      public override int MaxTextLength => 256000;

      public override bool isSpeakNativeSupported => false;

      public override bool isSpeakSupported => true;

      public override bool isSSMLSupported => true;

      public override bool isOnlineService => true;

      public override bool hasCoRoutines => true;

      public override bool isIL2CPPSupported => true;

      public override bool hasVoicesInEditor => false;

      /// <summary>Indicates if the API key is valid.</summary>
      /// <returns>True if the API key is valid.</returns>
      public bool isValidAPIKey => GCTextToSpeech.Instance.apiKey?.Length >= 32;

      #endregion


      #region MonoBehaviour methods

#if CT_DEVELOP
      private void Awake()
      {
         GCTextToSpeech.Instance.apiKey = APIKeys.Google;
      }
#endif

      private void Start()
      {
         GCTextToSpeech.Instance.GetVoicesSuccessEvent += onGetVoicesSuccessEvent;
         GCTextToSpeech.Instance.GetVoicesFailedEvent += onGetVoicesFailedEvent;
      }

      private void OnDestroy()
      {
         GCTextToSpeech.Instance.GetVoicesSuccessEvent -= onGetVoicesSuccessEvent;
         GCTextToSpeech.Instance.GetVoicesFailedEvent -= onGetVoicesFailedEvent;
      }

      #endregion


      #region Implemented methods

      public override void Load(bool forceReload = false)
      {
         if (cachedVoices?.Count == 0 || forceReload)
         {
            if (!isLoading)
            {
               isLoading = true;
               Invoke(nameof(load), 0.1f);
            }

            //GCTextToSpeech.Instance.GetVoices(new GetVoicesRequest());
            /*{
                languageCode = _gcTextToSpeech.PrepareLanguage((Enumerators.LanguageCode)languageCodesDropdown.value)
            });
            */
         }
         else
         {
            onVoicesReady();
         }
      }

      public override IEnumerator Generate(Model.Wrapper wrapper)
      {
#if !UNITY_WEBGL
         if (isReady(wrapper))
         {
            if (wrapper == null)
            {
               Debug.LogWarning("'wrapper' is null!", this);
            }
            else
            {
               if (string.IsNullOrEmpty(wrapper.Text))
               {
                  Debug.LogWarning("'wrapper.Text' is null or empty!", this);
               }
               else
               {
                  if (!Util.Helper.isInternetAvailable)
                  {
                     const string errorMessage = "Internet is not available - can't use Google Cloud Speech right now!";
                     Debug.LogError(errorMessage, this);
                     onErrorInfo(wrapper, errorMessage);
                  }
                  else
                  {
                     yield return null; //return to the main process (uid)
                     silence = false;
                     bool success = false;

                     onSpeakAudioGenerationStart(wrapper);

                     string outputFile = getOutputFile(wrapper.Uid, Util.Helper.isWebPlatform);
                     Model.Voice voice = getVoice(wrapper);

                     bool isWorking = true;
                     PostSynthesizeResponse response = null;

                     void Handler(PostSynthesizeResponse r, long requestId)
                     {
                        isWorking = false;
                        response = r;

                        GCTextToSpeech.Instance.SynthesizeSuccessEvent -= Handler;
                     }

                     GCTextToSpeech.Instance.SynthesizeSuccessEvent += Handler;

                     GCTextToSpeech.Instance.Synthesize(prepareProsody(wrapper), new VoiceConfig
                        {
                           gender = getGenderFromRTV(voice.Gender),
                           languageCode = voice.Culture,
                           name = voice.Name
                        },
                        wrapper.ForceSSML,
                        calculatePitch(wrapper.Pitch),
                        calculateRate(wrapper.Rate),
                        voice.SampleRate);

                     do
                     {
                        yield return null;
                     } while (isWorking);

                     try
                     {
                        System.IO.File.WriteAllBytes(outputFile, System.Convert.FromBase64String(response.audioContent));

                        success = true;
                     }
                     catch (System.Exception ex)
                     {
                        string errorMessage = "Could not create output file: " + outputFile + System.Environment.NewLine + "Error: " + ex;
                        Debug.LogError(errorMessage, this);
                        onErrorInfo(wrapper, errorMessage);
                     }

                     if (success)
                        processAudioFile(wrapper, outputFile);
                  }
               }
            }
         }
#else
         Debug.LogError("'Generate' is not supported under WebGL!", this);
         yield return null;
#endif
      }

      public override IEnumerator SpeakNative(Model.Wrapper wrapper)
      {
         yield return speak(wrapper, true);
      }

      public override IEnumerator Speak(Model.Wrapper wrapper)
      {
         yield return speak(wrapper, false);
      }

      #endregion


      #region Private methods

      private void load()
      {
         if (!Util.Helper.isInternetAvailable)
         {
            const string errorMessage = "Internet is not available - can't use Google Cloud Speech right now!";
            Debug.LogError(errorMessage, this);
            onErrorInfo(null, errorMessage);
         }
         else
         {
            if (!Util.Helper.isEditorMode && isReady(null))
               GCTextToSpeech.Instance.GetVoices(new GetVoicesRequest());
         }

         isLoading = false;
      }

      private Model.Voice getVoice(Model.Wrapper wrapper)
      {
         if (wrapper.Voice == null)
         {
            if (Util.Config.DEBUG)
               Debug.LogWarning("'wrapper.Voice' is null! Using the 'default' voice.", this);

            //always use 'DefaultVoiceName' as fallback
            return Speaker.Instance.VoiceForName(DefaultVoiceName);
         }

         return wrapper.Voice;
      }

      private bool isReady(Model.Wrapper wrapper, bool log = true)
      {
         if (!isValidAPIKey)
         {
            const string errorMessage = "Please add a valid 'API Key' to access Google Cloud Speech!";

            if (log)
               Debug.LogError(errorMessage, this);
            onErrorInfo(wrapper, errorMessage);

            return false;
         }

         return true;
      }

      private IEnumerator speak(Model.Wrapper wrapper, bool isNative)
      {
         if (isReady(wrapper))
         {
            if (wrapper == null)
            {
               Debug.LogWarning("'wrapper' is null!", this);
            }
            else
            {
               if (string.IsNullOrEmpty(wrapper.Text))
               {
                  Debug.LogWarning("'wrapper.Text' is null or empty!", this);
               }
               else
               {
                  if (!Util.Helper.isInternetAvailable)
                  {
                     const string errorMessage = "Internet is not available - can't use Azure right now!";
                     Debug.LogError(errorMessage, this);
                     onErrorInfo(wrapper, errorMessage);
                  }
                  else
                  {
                     yield return null; //return to the main process (uid)
                     silence = false;

                     if (!isNative)
                        onSpeakAudioGenerationStart(wrapper);

                     string outputFile = getOutputFile(wrapper.Uid, Util.Helper.isWebPlatform);
                     Model.Voice voice = getVoice(wrapper);

                     bool isWorking = true;
                     PostSynthesizeResponse response = null;

                     void Handler(PostSynthesizeResponse r, long requestId)
                     {
                        isWorking = false;
                        response = r;

                        GCTextToSpeech.Instance.SynthesizeSuccessEvent -= Handler;
                     }

                     GCTextToSpeech.Instance.SynthesizeSuccessEvent += Handler;

                     GCTextToSpeech.Instance.Synthesize(prepareProsody(wrapper), new VoiceConfig
                        {
                           gender = getGenderFromRTV(voice.Gender),
                           languageCode = voice.Culture,
                           name = voice.Name
                        },
                        wrapper.ForceSSML,
                        calculatePitch(wrapper.Pitch),
                        calculateRate(wrapper.Rate),
                        voice.SampleRate);

                     do
                     {
                        yield return null;
                     } while (isWorking);
#if UNITY_WEBGL
                     AudioClip ac = Common.Audio.WavMaster.ToAudioClip(System.Convert.FromBase64String(response.audioContent));
                     yield return playAudioFile(wrapper, ac, isNative);
#else
                     bool success = false;

                     try
                     {
                        System.IO.File.WriteAllBytes(outputFile, System.Convert.FromBase64String(response.audioContent));

                        success = true;
                     }
                     catch (System.Exception ex)
                     {
                        string errorMessage = "Could not create output file: " + outputFile + System.Environment.NewLine + "Error: " + ex;
                        Debug.LogError(errorMessage, this);
                        onErrorInfo(wrapper, errorMessage);
                     }

                     if (success)
                        yield return playAudioFile(wrapper, Util.Helper.ValidURLFromFilePath(outputFile), outputFile, AudioFileType, isNative);
#endif
                  }
               }
            }
         }
      }

      private static string prepareProsody(Model.Wrapper wrapper)
      {
         System.Text.StringBuilder sbXML = new System.Text.StringBuilder();

         sbXML.Append("<speak>");
         if (Mathf.Abs(wrapper.Volume - 1f) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
         {
            sbXML.Append("<prosody");
            float _volume = wrapper.Volume;

            sbXML.Append(" volume=\"");

            sbXML.Append(_volume.ToString("#0.00", Util.Helper.BaseCulture));

            sbXML.Append("\"");

            sbXML.Append(">");

            sbXML.Append(wrapper.Text);

            sbXML.Append("</prosody>");
         }
         else
         {
            sbXML.Append(wrapper.Text);
         }

         sbXML.Append("</speak>");

         //Debug.Log(sbXML);

         return sbXML.ToString();
      }

      private static Model.Enum.Gender getGenderFromGC(Enumerators.SsmlVoiceGender gender)
      {
         switch (gender)
         {
            case Enumerators.SsmlVoiceGender.FEMALE:
               return Model.Enum.Gender.FEMALE;
            case Enumerators.SsmlVoiceGender.MALE:
               return Model.Enum.Gender.MALE;
            default:
               return Model.Enum.Gender.UNKNOWN;
         }
      }

      private static Enumerators.SsmlVoiceGender getGenderFromRTV(Model.Enum.Gender gender)
      {
         switch (gender)
         {
            case Model.Enum.Gender.FEMALE:
               return Enumerators.SsmlVoiceGender.FEMALE;
            case Model.Enum.Gender.MALE:
               return Enumerators.SsmlVoiceGender.MALE;
            default:
               return Enumerators.SsmlVoiceGender.NEUTRAL;
         }
      }

      private static float calculateRate(float rate)
      {
         return Mathf.Clamp(rate, 0.25f, 4f);
      }

      private static float calculatePitch(float pitch)
      {
         float result = 0;

         if (Mathf.Abs(pitch - 1f) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
         {
            if (pitch > 1f)
            {
               result = (int)((pitch - 1f) * 20f);
            }
            else
            {
               result = -(int)((1f - pitch) * 20f);
            }
         }

         if (Util.Constants.DEV_DEBUG)
            Debug.Log($"calculatePitch: {result} ({pitch})");

         return result;
      }

      #endregion


      #region Callbacks

      private void onGetVoicesSuccessEvent(GetVoicesResponse response, long requestId)
      {
         //Debug.Log("Voices: " + response.voices.CTDump(), this);

         System.Collections.Generic.List<Model.Voice> voices = new System.Collections.Generic.List<Model.Voice>(response.voices.Length);
         voices.AddRange(response.voices.Select(voice => new Model.Voice(voice.name, voice.name, getGenderFromGC(voice.ssmlGender), "unknown", string.Concat(voice.languageCodes), "", "Google", "unknown", (int)voice.naturalSampleRateHertz)));

         cachedVoices = voices.OrderBy(s => s.Name).ToList();

         onVoicesReady();
      }

      private void onGetVoicesFailedEvent(string error, long requestId)
      {
         onErrorInfo(null, error);
         Debug.LogError(error, this);
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR
      public override void GenerateInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'GenerateInEditor' is not supported for Google Cloud Speech!", this);
      }

      public override void SpeakNativeInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'SpeakNativeInEditor' is not supported for Google Cloud Speech!", this);
      }
#endif

      #endregion
   }
}
// © 2019-2021 crosstales LLC (https://www.crosstales.com)