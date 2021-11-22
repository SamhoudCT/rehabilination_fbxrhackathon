using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using FrostweepGames.Plugins.Core;

namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public class GC_TextToSpeech_TutorialExample : MonoBehaviour
    {
        private GCTextToSpeech _gcTextToSpeech;

        private Voice[] _voices;
        private Voice _currentVoice;

        private CultureInfo _provider;

        public Button synthesizeButton;
        public Button getVoicesButton;

        public InputField contentInputFioeld;
        public InputField pitchInputField;
        public InputField speakingRateInputField;

        public Toggle ssmlToggle;

        public Dropdown languageCodesDropdown;
        public Dropdown voiceTypesDropdown;
        public Dropdown voicesDropdown;

        public AudioSource audioSource;

        private void Start()
        {
            _gcTextToSpeech = GCTextToSpeech.Instance;

            _gcTextToSpeech.GetVoicesSuccessEvent += _gcTextToSpeech_GetVoicesSuccessEvent; 
            _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;

            _gcTextToSpeech.GetVoicesFailedEvent += _gcTextToSpeech_GetVoicesFailedEvent;
            _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;

            synthesizeButton.onClick.AddListener(SynthesizeButtonOnClickHandler);
            getVoicesButton.onClick.AddListener(GetVoicesButtonOnClickHandler);

            voicesDropdown.onValueChanged.AddListener(VoiceSelectedDropdownOnChangedHandler);
            voiceTypesDropdown.onValueChanged.AddListener(VoiceTypeSelectedDropdownOnChangedHandler);

            _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _provider.NumberFormat.NumberDecimalSeparator = ".";


            int length = Enum.GetNames(typeof(Enumerators.LanguageCode)).Length;
            List<string> elements = new List<string>();

            for(int i = 0; i < length; i++)
                elements.Add(((Enumerators.LanguageCode)i).ToString());

            languageCodesDropdown.ClearOptions();
            languageCodesDropdown.AddOptions(elements);
            languageCodesDropdown.value = length-1;

            length = Enum.GetNames(typeof(Enumerators.VoiceType)).Length;
            elements.Clear();

            for (int i = 0; i < length; i++)
                elements.Add(((Enumerators.VoiceType)i).ToString());

            voiceTypesDropdown.ClearOptions();
            voiceTypesDropdown.AddOptions(elements);
            voiceTypesDropdown.value = 0;

            GetVoicesButtonOnClickHandler();
        }

        private void SynthesizeButtonOnClickHandler()
        {
            string content = contentInputFioeld.text;

            if (string.IsNullOrEmpty(content) || _currentVoice == null)
                return;

            _gcTextToSpeech.Synthesize(content, new VoiceConfig()
            {
                gender = _currentVoice.ssmlGender,
                languageCode = _currentVoice.languageCodes[0],
                name = _currentVoice.name
            },
            ssmlToggle.isOn,
            double.Parse(pitchInputField.text, _provider),
            double.Parse(speakingRateInputField.text, _provider),
            _currentVoice.naturalSampleRateHertz);
        }

        private void GetVoicesButtonOnClickHandler()
        {
            _gcTextToSpeech.GetVoices(new GetVoicesRequest()
            {
                languageCode =  _gcTextToSpeech.PrepareLanguage((Enumerators.LanguageCode)languageCodesDropdown.value)
            });
        }


        private void FillVoicesList()
        {
            if (_voices == null)
                return;

            List<string> elements = new List<string>();

            for (int i = 0; i < _voices.Length; i++)
            {
                if (_voices[i].name.ToLower().Contains(((Enumerators.VoiceType)voiceTypesDropdown.value).ToString().ToLower()))
                    elements.Add(_voices[i].name);
            }

            voicesDropdown.ClearOptions();
            voicesDropdown.AddOptions(elements);

            if (elements.Count > 0)
            {
                voicesDropdown.value = 0;
                VoiceSelectedDropdownOnChangedHandler(0);
            }
        }

        private void VoiceSelectedDropdownOnChangedHandler(int index)
        {
            var voice = _voices.ToList().Find(item => item.name.Contains(voicesDropdown.options[index].text));
            _currentVoice = voice;
        }

        private void VoiceTypeSelectedDropdownOnChangedHandler(int index)
        {
            FillVoicesList();
        }

        #region failed handlers

        private void _gcTextToSpeech_SynthesizeFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        private void _gcTextToSpeech_GetVoicesFailedEvent(string error, long requestId)
        {
            Debug.Log(error);
        }

        #endregion failed handlers

        #region sucess handlers

        private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response, long requestId)
        {
            audioSource.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
            audioSource.Play();
			
            // SAVE FILE TO LOCAL STORAGE
			/* ServiceLocator.Get<IMediaManager>().
				SaveAudioFileAsFile(response.audioContent, 
									Application.dataPath, 
									"sound_" + System.DateTime.Now.ToString(), 
									Enumerators.AudioEncoding.LINEAR16); */
        }

        private void _gcTextToSpeech_GetVoicesSuccessEvent(GetVoicesResponse response, long requestId)
        {
            _voices = response.voices;

            FillVoicesList();
        }


        #endregion sucess handlers
    }
}