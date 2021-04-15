using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapticPlugin;
using System;

namespace AppCore
{
    public enum TapticType
    {
        ImpactLight,
        ImpactMedium,
        ImpactHeavy,

        NotificationSuccess,
        NotificationWarning,
        NotificationError,

        Selection,
    }

    public static class CrossPlatformTaptic
    {
        public static void Impact(ImpactFeedback impactFeedback)
        {
#if UNITY_IOS
            TapticManager.Impact(impactFeedback);
#elif UNITY_ANDROID
            AndroidHapticManager.Impact(impactFeedback);
#endif
        }

        public static void Notification(NotificationFeedback notificationFeedback)
        {
#if UNITY_IOS
            TapticManager.Notification(notificationFeedback);
#elif UNITY_ANDROID
            AndroidHapticManager.Notification(notificationFeedback);
#endif
        }

        public static void Selection()
        {
#if UNITY_IOS
            TapticManager.Selection();
#elif UNITY_ANDROID
            AndroidHapticManager.Selection();
#endif
        }

        public static T Convert<T>(TapticType tapticType) where T : struct, IConvertible
        {
            int value;

            switch (tapticType)
            {
                case TapticType.ImpactLight:
                    value = (int)ImpactFeedback.Light;
                    break;

                case TapticType.ImpactMedium:
                    value = (int)ImpactFeedback.Medium;
                    break;

                case TapticType.ImpactHeavy:
                    value = (int)ImpactFeedback.Heavy;
                    break;

                case TapticType.NotificationSuccess:
                    value = (int)NotificationFeedback.Success;
                    break;

                case TapticType.NotificationWarning:
                    value = (int)NotificationFeedback.Warning;
                    break;

                case TapticType.NotificationError:
                    value = (int)NotificationFeedback.Error;
                    break;

                case TapticType.Selection:
                default:
                    return default;
            }

            return (T)(object)value;
        }

        public static void Call(TapticType tapticType)
        {
            switch (tapticType)
            {
                case TapticType.ImpactHeavy:
                case TapticType.ImpactMedium:
                case TapticType.ImpactLight:
                    Impact(Convert<ImpactFeedback>(tapticType));
                    break;

                case TapticType.NotificationSuccess:
                case TapticType.NotificationWarning:
                case TapticType.NotificationError:
                    Notification(Convert<NotificationFeedback>(tapticType));
                    break;

                case TapticType.Selection:
                    Selection();
                    break;
            }
        }
    }
}