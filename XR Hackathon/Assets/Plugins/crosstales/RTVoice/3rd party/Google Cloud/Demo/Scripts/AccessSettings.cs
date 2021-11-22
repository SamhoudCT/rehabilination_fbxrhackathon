using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

namespace Crosstales.RTVoice.Google
{
   /// <summary>Set the access settings for Google Cloud Speech.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_google_1_1_access_settings.html")]
   public class AccessSettings : MonoBehaviour
   {
      #region Variables

      public GameObject SettingsPanel;

      public InputField APIKey;

      public Button OkButton;

      private string enteredKey = string.Empty;

      private static string lastKey;

      private Color okColor;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         okColor = OkButton.image.color;

         if (!string.IsNullOrEmpty(lastKey))
            GCTextToSpeech.Instance.apiKey = lastKey;

         if (string.IsNullOrEmpty(GCTextToSpeech.Instance.apiKey))
         {
            ShowSettings();
         }
         else
         {
            HideSettings();
            enteredKey = lastKey = APIKey.text = GCTextToSpeech.Instance.apiKey;
         }

         SetOkButton();
      }

      #endregion


      #region Public methods

      public void OnAPIKeyEntered(string key)
      {
         enteredKey = key ?? string.Empty;
         SetOkButton();
      }

      public void HideSettings()
      {
         SettingsPanel.SetActive(false);

         if (!string.IsNullOrEmpty(enteredKey) && !enteredKey.Equals(lastKey))
         {
            lastKey = GCTextToSpeech.Instance.apiKey = enteredKey;
            Speaker.Instance.ReloadProvider();
         }
      }

      public void ShowSettings()
      {
         SettingsPanel.SetActive(true);
      }

      public void SetOkButton()
      {
         if (enteredKey.Length >= 32)
         {
            OkButton.interactable = true;
            OkButton.image.color = okColor;
         }
         else
         {
            OkButton.interactable = false;
            OkButton.image.color = Color.gray;
         }
      }

      #endregion
   }
}
// © 2020-2021 crosstales LLC (https://www.crosstales.com)