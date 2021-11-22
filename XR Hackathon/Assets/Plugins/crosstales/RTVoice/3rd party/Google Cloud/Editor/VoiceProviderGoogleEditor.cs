#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.RTVoice.Google
{
   /// <summary>Custom editor for the 'VoiceProviderGoogle'-class.</summary>
   [CustomEditor(typeof(VoiceProviderGoogle))]
   public class VoiceProviderGoogleEditor : Editor
   {
      #region Variables

      private VoiceProviderGoogle script;

      //private string apiKey = string.Empty;
      //private string requestUri = string.Empty;

      #endregion


      #region Properties

      public static bool isPrefabInScene => GameObject.Find("Google Cloud") != null;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (VoiceProviderGoogle)target;
      }

      public override void OnInspectorGUI()
      {
         EditorUtil.EditorHelper.BannerOC();

         //DrawDefaultInspector();

         if (GUILayout.Button(new GUIContent(" Learn more", EditorUtil.EditorHelper.Icon_Manual, "Learn more about Google Cloud Speech.")))
            Util.Helper.OpenURL(Crosstales.RTVoice.Util.Constants.ASSET_3P_GOOGLE);

         //EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (script.isPlatformSupported)
            {
               if (script.isValidAPIKey)
               {
                  //add stuff if needed
               }
               else
               {
                  EditorUtil.EditorHelper.SeparatorUI();
                  EditorGUILayout.HelpBox("Please add a valid 'API Key' to access Google Cloud Speech!", MessageType.Warning);
               }
            }
            else
            {
               EditorUtil.EditorHelper.SeparatorUI();
               EditorGUILayout.HelpBox("The current platform is not supported by Google Cloud Speech. Please use MaryTTS or a custom provider (e.g. Klattersynth).", MessageType.Error);
            }
         }
         else
         {
            EditorUtil.EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2019-2021 crosstales LLC (https://www.crosstales.com)