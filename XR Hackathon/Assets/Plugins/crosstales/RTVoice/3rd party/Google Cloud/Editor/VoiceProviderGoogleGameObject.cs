#if UNITY_EDITOR
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.Google
{
   /// <summary>Editor component for for adding the prefabs from 'Google' in the "Hierarchy"-menu.</summary>
   public static class VoiceProviderGoogleGameObject
   {
      [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/3rd party/VoiceProviderGoogle", false, EditorUtil.EditorHelper.GO_ID + 40)]
      private static void AddVoiceProvider()
      {
         EditorHelper.InstantiatePrefab("Google Cloud", $"{EditorConfig.ASSET_PATH}3rd party/Google Cloud/Prefabs/");
      }

      [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/3rd party/VoiceProviderGoogle", true)]
      private static bool AddVoiceProviderValidator()
      {
         return !VoiceProviderGoogleEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2019-2021 crosstales LLC (https://www.crosstales.com)