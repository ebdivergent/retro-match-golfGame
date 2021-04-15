using System;
using System.Collections.Generic;
using AppCore;
using UnityEngine;

namespace AppCore
{
    public interface IAudioStream : IAudioSettingsContainer
    {
        string ID { get; }
        bool Loop { get; }
        AudioClip Clip { get; }
        IAudioGroup Group { get; }
        OverrideSettingsFlag OverrideGroupFlags { get; }
    }

    public class AudioStream : MonoBehaviour, IAudioStream
    {
        private bool _pause;

        public static AudioStream.Builder GetBuilder(AudioSource source)
        {
            var builder = new AudioStream.Builder(AudioStream.CreateFromSource(source));

            return builder;
        }

        public static AudioStream.Builder GetBuilder()
        {
            return AudioStreamManager.Instance.GetStreamBuilder();
        }

        public static IAudioStream Play(AudioClip clip,
            AudioSource source = null,
            float? volume = null,
            IAudioGroup group = null,
            string id = null,
            bool? loop = null,
            bool? mute = null,
            bool? pause = null,
            OverrideSettingsFlag? overrideGroupFlags = null,
            Action onComplete = null)
        {
            var builder = source ? GetBuilder(source) : GetBuilder();

            builder.SetClip(clip);

            if (group != null)
                builder.SetGroup(group);

            if (id != null)
                builder.SetID(id);

            if (loop.HasValue)
                builder.SetLoop(loop.Value);

            if (volume.HasValue)
                builder.SetVolume(volume.Value);

            if (mute.HasValue)
                builder.SetMute(mute.Value);

            if (pause.HasValue)
                builder.SetPause(pause.Value);

            if (overrideGroupFlags.HasValue)
                builder.SetOverrideGroupFlags(overrideGroupFlags.Value);

            if (onComplete != null)
                builder.SetOnComplete(onComplete);

            return builder.Play();
        }
        
        public static AudioStream CreateFromSource(AudioSource source)
        {
            var stream = source.gameObject.AddComponent<AudioStream>();
            stream._settings = new AudioSettingsLayer();
            stream._source = source;
            return stream;
        }

        #region Private members
        [SerializeField] string _id = null;
        [SerializeField] AudioSource _source = null;
        [SerializeField] AudioSettingsLayer _settings = new AudioSettingsLayer();
        [SerializeField] IAudioGroup _group = null;
        [SerializeField] OverrideSettingsFlag _overrideGroupFlags = OverrideSettingsFlag.None;

        private List<Action> _onComplete = new List<Action>();

        public string ID { get { return _id; } set { _id = value; } }
        public bool Loop { get { return _source.loop; } set { _source.loop = value; } }
        public AudioClip Clip { get { return _source.clip; } set { _source.clip = value; } }
        public IAudioSettingsLayer Settings { get { return _settings; } }
        public AudioSource Source { get { return _source; } }
        public IAudioGroup Group { get { return _group; } }
        public OverrideSettingsFlag OverrideGroupFlags { get { return _overrideGroupFlags; } set { _overrideGroupFlags = value; InternalUpdate(); } }
        #endregion

        #region Unity interface
        private void Reset()
        {
            UnsubscribeSettings();

            SetGroup(null);

            _pause = false;
            _id = null;
            _settings.Reset();
            _onComplete?.Clear();

            _source.loop = false;
            _source.clip = null;
            
            _overrideGroupFlags = OverrideSettingsFlag.None;

            SubscribeSettings();

            InternalUpdate();
        }

        private void Update()
        {
            if (!_pause && !_source.isPlaying)
            {
                InvokeComplete();
            }
        }

        private void OnDisable()
        {
            Reset();
        }

        private void OnDestroy()
        {
            UnsubscribeSettings();
        }
        #endregion


        #region Event handlers
        private void OnMute(bool mute)
        {
            bool totalMute = _settings.Mute || AudioSettingsManager.Instance.Settings.Mute;

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Mute))
            {
                totalMute |= _group.Settings.Mute;
            }

            _source.mute = totalMute;
        }

        private void OnPause(bool pause)
        {
            bool totalPause = _settings.Pause || AudioSettingsManager.Instance.Settings.Pause;

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Pause))
            {
                totalPause |= _group.Settings.Pause;
            }

            _pause = totalPause;

            if (_pause)
            {
                _source.Pause();
            }
            else
            {
                _source.UnPause();
            }
        }

        private void OnVolumeChanged(float volume)
        {
            float totalVolume = _settings.Volume * AudioSettingsManager.Instance.Settings.Volume;

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Volume))
            {
                totalVolume *= _group.Settings.Volume;
            }

            _source.volume = totalVolume;
        }
        #endregion

        #region Private methods
        private void SetGroup(IAudioGroup group)
        {
            if (_group != null)
            {
                _group.Settings.OnMute -= OnMute;
                _group.Settings.OnPause -= OnPause;
                _group.Settings.OnVolumeChanged -= OnVolumeChanged;

                (_group as AudioGroup).Remove(this);
            }

            if ((_group = group) != null)
            {
                (_group as AudioGroup).Add(this);

                _group.Settings.OnMute += OnMute;
                _group.Settings.OnPause += OnPause;
                _group.Settings.OnVolumeChanged += OnVolumeChanged;
            }
        }

        private void SubscribeSettings()
        {
            _settings.OnMute += OnMute;
            _settings.OnPause += OnPause;
            _settings.OnVolumeChanged += OnVolumeChanged;

            AudioSettingsManager.Instance.Settings.OnMute += OnMute;
            AudioSettingsManager.Instance.Settings.OnPause += OnPause;
            AudioSettingsManager.Instance.Settings.OnVolumeChanged += OnVolumeChanged;
        }

        private void UnsubscribeSettings()
        {
            _settings.OnMute -= OnMute;
            _settings.OnPause -= OnPause;
            _settings.OnVolumeChanged -= OnVolumeChanged;

            AudioSettingsManager.Instance.Settings.OnMute -= OnMute;
            AudioSettingsManager.Instance.Settings.OnPause -= OnPause;
            AudioSettingsManager.Instance.Settings.OnVolumeChanged -= OnVolumeChanged;
        }

        private void InternalUpdate()
        {
            float volume = _settings.Volume * AudioSettingsManager.Instance.Settings.Volume;
            bool mute = _settings.Mute || AudioSettingsManager.Instance.Settings.Mute;
            bool pause = _settings.Pause || AudioSettingsManager.Instance.Settings.Pause;

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Volume))
            {
                volume *= _group.Settings.Volume;
            }

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Mute))
            {
                mute |= _group.Settings.Mute;
            }

            if (_group != null && !_overrideGroupFlags.HasFlag(OverrideSettingsFlag.Pause))
            {
                pause |= _group.Settings.Pause;
            }

            _source.volume = volume;
            _source.mute = mute;
            _pause = pause;

            if (_pause)
            {
                _source.Pause();
            }
            else
            {
                _source.UnPause();
            }
        }

        private void InvokeComplete()
        {
            var onCompleteHandlers = new List<Action>(_onComplete);

            foreach (var handler in onCompleteHandlers)
            {
                handler?.Invoke();
            }

            _onComplete.Clear();
        }
        #endregion

        #region Public methods
        public void Play()
        {
            _source.Play();
        }

        public void Stop()
        {
            _source.Stop();

            InvokeComplete();
        }

        public void SetOnComplete(Action onComplete)
        {
            _onComplete.Add(onComplete);
        }
        #endregion

        #region Builder
        public class Builder : IDisposable
        {
            private AudioStream _stream;

            public Builder (AudioSource source)
            {
                _stream = CreateFromSource(source);
            }

            public Builder(IAudioStream stream)
            {
                _stream = (stream as AudioStream);
                _stream.Reset();
            }

            public IAudioStream Play()
            {
                if (_stream._group == null)
                {
                    _stream.SetGroup(AudioGroupManager.Instance.DefaultGroup);
                }

                _stream.SubscribeSettings();
                //_stream.InternalUpdate();
                _stream.Play();
                _stream.InternalUpdate();

                var stream = _stream;

                _stream = null;

                return stream;
            }

            public Builder SetGroup(IAudioGroup group)
            {
                _stream.SetGroup(group);
                return this;
            }

            public Builder SetID(string id)
            {
                _stream._id = id;
                return this;
            }

            public Builder SetClip(AudioClip clip)
            {
                _stream._source.clip = clip;
                return this;
            }

            public Builder SetLoop(bool loop)
            {
                _stream._source.loop = loop;
                return this;
            }

            public Builder SetVolume(float volume)
            {
                _stream._settings.Volume = volume;
                return this;
            }

            public Builder SetMute(bool mute)
            {
                _stream._settings.Mute = mute;
                return this;
            }

            public Builder SetPause(bool pause)
            {
                _stream._settings.Pause = pause;
                return this;
            }

            public Builder SetOnComplete(Action onComplete)
            {
                _stream._onComplete.Add(onComplete);
                return this;
            }

            public Builder SetOverrideGroupFlags(OverrideSettingsFlag overrideSettingsFlag)
            {
                _stream.OverrideGroupFlags = overrideSettingsFlag;
                return this;
            }

            public void Dispose()
            {
                if (_stream != null)
                {
                    var poolable = _stream.GetComponent<Pooling.PoolableElement>();

                    if (poolable)
                    {
                        poolable.Return();
                    }

                    _stream = null;
                }
            }

            ~Builder()
            {
                Dispose();
            }
        }
        #endregion
    }
}
