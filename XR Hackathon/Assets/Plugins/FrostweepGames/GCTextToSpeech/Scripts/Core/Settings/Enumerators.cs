namespace FrostweepGames.Plugins.GoogleCloud.TextToSpeech
{
    public class Enumerators
    {
        public enum GoogleCloudRequestType
        {
            GET_VOICES,
            SYNTHESIZE
        }

        public enum SsmlVoiceGender
        {
            SSML_VOICE_GENDER_UNSPECIFIED,
            MALE,
            FEMALE,
            NEUTRAL
        }

        public enum AudioEncoding
        {
            AUDIO_ENCODING_UNSPECIFIED,
            LINEAR16,
            MP3,
            OGG_OPUS
        }

        public enum LanguageCode
        {
            af_ZA,
            ar_XA,
            bn_IN,
            bg_BG,
            ca_ES,
            yue_HK,
            cs_CZ,
            da_DK,
            nl_BE,
            nl_NL,
            en_AU,
            en_IN,
            en_GB,
            en_US,
            fil_PH,
            fi_FI,
            fr_CA,
            fr_FR,
            de_DE,
            el_GR,
            gu_IN,
            hi_IN,
            hu_HU,
            is_IS,
            id_ID,
            it_IT,
            ja_JP,
            kn_IN,
            ko_KR,
            lv_LV,
            ms_MY,
            ml_IN,
            cmn_CN,
            cmn_TW,
            nb_NO,
            pl_PL,
            pt_BR,
            pt_PT,
            ro_RO,
            ru_RU,
            sr_RS,
            sk_SK,
            es_ES,
            es_US,
            sv_SE,
            ta_IN,
            te_IN,
            th_TH,
            tr_TR,
            uk_UA,
            vi_VN
        }

        public enum VoiceType
        {
            WAVENET,
            STANDARD
        }
    }
}