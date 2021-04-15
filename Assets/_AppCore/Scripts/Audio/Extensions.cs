using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public static class AudioGroupExtensions
    {
        //public static IAudioGroup SetID(this IAudioGroup group, string id)
        //{
        //    if (group == null)
        //    {
        //        Debug.LogWarning("Group not found.");
        //        return null;
        //    }

        //    (group as AudioGroup).ID = id;

        //    return group;
        //}
    }

    public static class AudioStreamExtensions
    {
        public static IAudioStream SetID(this IAudioStream stream, string id)
        {
            if (stream == null)
            {
                Debug.LogWarning("Stream not found.");
                return null;
            }

            (stream as AudioStream).ID = id;

            return stream;
        }

        public static IAudioStream SetOnComplete(this IAudioStream stream, Action onComplete)
        {
            if (stream == null)
            {
                Debug.LogWarning("Stream not found.");
                return null;
            }

            (stream as AudioStream).SetOnComplete(onComplete);

            return stream;
        }

        public static IAudioStream SetOverrideGroupFlags(this IAudioStream stream, OverrideSettingsFlag overrideSettingsFlag)
        {
            if (stream == null)
            {
                Debug.LogWarning("Stream not found.");
                return null;
            }

            (stream as AudioStream).OverrideGroupFlags = overrideSettingsFlag;

            return stream;
        }

        public static void Stop(this IAudioStream stream)
        {
            if (stream == null)
            {
                Debug.LogWarning("Stream not found.");
                return;
            }

            (stream as AudioStream).Stop();
        }
    }

    public static class AudioContainerExtensions
    {
        public static T SetVolume<T>(this T settingsContainer, float volume) where T : IAudioSettingsContainer
        {
            if (settingsContainer == null)
            {
                Debug.LogWarning("Container not found.");
                return default;
            }

            var settings = (settingsContainer.Settings as AudioSettingsLayer);

            settings.Volume = volume;

            return settingsContainer;
        }

        public static T SetMute<T>(this T settingsContainer, bool mute) where T : IAudioSettingsContainer
        {
            if (settingsContainer == null)
            {
                Debug.LogWarning("Container not found.");
                return default;
            }

            var settings = (settingsContainer.Settings as AudioSettingsLayer);

            settings.Mute = mute;

            return settingsContainer;
        }

        public static T SetPause<T>(this T settingsContainer, bool pause) where T : IAudioSettingsContainer
        {
            if (settingsContainer == null)
            {
                Debug.LogWarning("Container not found.");
                return default;
            }

            var settings = (settingsContainer.Settings as AudioSettingsLayer);

            settings.Pause = pause;

            return settingsContainer;
        }

        public static T TweenVolume<T>(this T settingsContainer, float volume, float duration, Action<DG.Tweening.Tweener> tweenerAction = null) where T : IAudioSettingsContainer
        {
            if (settingsContainer == null)
            {
                Debug.LogWarning("Container not found.");
                return default;
            }

            var settings = (settingsContainer.Settings as AudioSettingsLayer);

            var tweener = settings.TweenVolume(volume, duration);

            tweenerAction?.Invoke(tweener);

            return settingsContainer;
        }

        public static T StopTween<T>(this T settingsContainer) where T : IAudioSettingsContainer
        {
            if (settingsContainer == null)
            {
                Debug.LogWarning("Container not found.");
                return default;
            }

            var settings = (settingsContainer.Settings as AudioSettingsLayer);

            settings.StopTween();

            return settingsContainer;
        }
    }
}
