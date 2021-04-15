using TapticPlugin;
using UnityEngine;

namespace AppCore
{
    public class AndroidHapticManager
    {
        //private static int s_hapticFeedbackConstantsKey;
        //private static AndroidJavaObject s_unityPlayer;

        static AndroidHapticManager()
        {
            //s_hapticFeedbackConstantsKey = new AndroidJavaClass("android.view.HapticFeedbackConstants")
            //    .GetStatic<int>("VIRTUAL_KEY");

            //s_unityPlayer =
            //    new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            //    .GetStatic<AndroidJavaObject>("currentActivity")
            //    .Get<AndroidJavaObject>("mUnityPlayer");
        }

        public static void Impact(ImpactFeedback impactFeedback)
        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//                return UnityPlayer.Call<bool> ("performHapticFeedback",HapticFeedbackConstantsKey);
//#endif
        }

        public static void Notification(NotificationFeedback notificationFeedback)
        {
            //#if UNITY_ANDROID && !UNITY_EDITOR
            //                return UnityPlayer.Call<bool> ("performHapticFeedback",HapticFeedbackConstantsKey);
            //#endif
        }

        public static void Selection()
        {
            //#if UNITY_ANDROID && !UNITY_EDITOR
            //                return UnityPlayer.Call<bool> ("performHapticFeedback",HapticFeedbackConstantsKey);
            //#endif
        }
    }
}