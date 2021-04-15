using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public interface IAudioGroup : IAudioSettingsContainer
    {
        string ID { get; }

        IAudioStream GetStream(string id);
        IAudioStream GetStreamByClip(AudioClip clip);
        IAudioStream GetStreamByClip(string clipName);

        IAudioStream[] GetStreams(string id);
        IAudioStream[] GetStreamsByClip(AudioClip clip);
        IAudioStream[] GetStreamsByClip(string clipName);
    }

    [Serializable]
    public class AudioGroup : IAudioGroup
    {
        #region Private members
        [SerializeField] string _id;
        [SerializeField] List<IAudioStream> _streams = new List<IAudioStream>();
        [SerializeField] AudioSettingsLayer _settings = new AudioSettingsLayer();
        #endregion

        #region Properties
        public string ID { get { return _id; } set { _id = value; } }
        public IAudioSettingsLayer Settings { get { return _settings; } }
        #endregion

        public AudioGroup(string id = null)
        {
            _id = id;
        }

        #region Public methods
        public void Add(IAudioStream stream)
        {
            _streams.Add(stream);
        }

        public bool Remove(IAudioStream stream)
        {
            return _streams.Remove(stream);
        }

        public IAudioStream GetStream(string id)
        {
            if (id == null)
                return null;

            foreach (var stream in _streams)
            {
                if (stream.ID == id)
                    return stream;
            }

            return null;
        }

        public IAudioStream GetStreamByClip(AudioClip clip)
        {
            if (clip == null)
                return null;

            foreach (var stream in _streams)
            {
                if (stream.Clip == clip)
                    return stream;
            }

            return null;
        }

        public IAudioStream GetStreamByClip(string clipName)
        {
            if (clipName == null)
                return null;

            foreach (var stream in _streams)
            {
                if (stream.Clip?.name == clipName)
                    return stream;
            }

            return null;
        }

        public IAudioStream[] GetStreams(string id)
        {
            if (id == null)
                return null;

            var list = new List<IAudioStream>();

            foreach (var stream in _streams)
            {
                if (stream.ID == id)
                    list.Add(stream);
            }

            return list.ToArray();
        }

        public IAudioStream[] GetStreamsByClip(AudioClip clip)
        {
            if (clip == null)
                return null;

            var list = new List<IAudioStream>();

            foreach (var stream in _streams)
            {
                if (stream.Clip == clip)
                    list.Add(stream);
            }

            return list.ToArray();
        }

        public IAudioStream[] GetStreamsByClip(string clipName)
        {
            if (clipName == null)
                return null;

            var list = new List<IAudioStream>();

            foreach (var stream in _streams)
            {
                if (stream.Clip?.name == clipName)
                    list.Add(stream);
            }

            return list.ToArray();
        }
        #endregion
    }
}